using DataTruck.Comparers;
using DataTruck.Configuration;
using DataTruck.Core.Models;
using DataTruck.Interfaces;
using DataTruck.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DataTruck.Core.Services;
using DataTruck.Data;
using DataTruck.DbManagers;
using Microsoft.EntityFrameworkCore;

namespace DataTruck.Core
{
    public class Runner : IRunner
    {
        private readonly ILogger<Runner> _logger;
        private readonly MigrationOptions _options;
        private readonly IFileProviderEx _fileProvider;
        private readonly IDbServer _server;
        private readonly IHashService _hashService;
        private readonly MigrationResult _migrationResult;
        private readonly IFileNameComparer _fileNameComparer;

        public Runner(ILogger<Runner> logger, IOptions<MigrationOptions> options,
            IFileProviderEx fileProvider, IDbServer server, IFileNameComparer fileNameComparer,
            IHashService hashService,
            MigrationResult migrationResult)
        {
            _logger = logger;
            _options = options.Value;

            _fileProvider = fileProvider;
            _fileNameComparer = fileNameComparer;
            _server = server;
            _hashService = hashService;

            _migrationResult = migrationResult;
            _migrationResult.EnableExitCodeError = _options.EnableExitCodeError;

        }

        /// <summary>
        /// Used in environment without dependency injection
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Runner Create(MigrationOptions options)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());

            // Construct EF DB context
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            DataContextOptionsBuilder.Configure(optionsBuilder, options.DbServer, options.ConnectionString);
            var context = new DataContext(optionsBuilder.Options);

            IDbServer serverMgr = options.DbServer switch
            {
                // TODO: Add here when new DB engines are supported
                DataContextOptionsBuilder.SqlServer => new SqlServerManager(loggerFactory.CreateLogger<SqlServerManager>(), context),
                DataContextOptionsBuilder.Oracle => new OracleManager(loggerFactory.CreateLogger<OracleManager>(), context),

                DataContextOptionsBuilder.InMemory => new MockServerManager(loggerFactory.CreateLogger<MockServerManager>(), context),
                // Unsupported DbEngines throws inconditionally
                _ => throw new Exception($"Invalid DB: {options.DbServer}")
            };

            return new Runner(
                loggerFactory.CreateLogger<Runner>(),
                Options.Create(options),
                new PhysicalFileProviderEx(options.SqlFilesPath),
                serverMgr,
                new CustomFileNameComparer(),
                new HashService(),
                new MigrationResult());
        }

        public async Task<MigrationResult> Run(CancellationToken stoppingToken = default)
        {
            _logger.LogInformation("Checking system tables...");
            try
            {
                await _server.Initialize(stoppingToken);
            } catch (Exception e)
            {
                _logger.LogError(e, "Error migrating DataTruck management tables");
                throw;
            }

            // Clean previous result!
            _migrationResult.Clear();

            // Start process
            _logger.LogInformation($"Migrating database for environment: {_options.DbContext}");

            Regex enableEnvMatch = new Regex($"[^a-zA-Z-0-9]env[^a-zA-Z-0-9]");
            Regex envMatch = new Regex($"[^a-zA-Z-0-9]{_options.DbContext}[^a-zA-Z-0-9]");

            Dictionary<string, List<IFileInfo>> dirs = new();

            var test = _fileProvider.GetDirectoryContents("/").Where(f => f.IsDirectory);
            foreach (var t in test)
            {
                List<IFileInfo> list = new();
                GetFiles(_fileProvider, t.Name, list);
                dirs.Add(t.Name, list);
            }

            var directories = _options.Directories.OrderBy(d => d.Order);
            foreach (var dir in directories)
            {
                if (!dirs.ContainsKey(dir.Name))
                {
                    _logger.LogInformation($"Directory not found: {dir.Name}");
                    continue;
                }

                var contents = dirs[dir.Name].OrderBy(f => f.Name, _fileNameComparer);

                foreach (var file in contents)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("STOP REQUESTED");
                        break;
                    }

                    _logger.LogInformation($"Checking: {file.Name}");

                    var migrationState = new MigrationState
                    {
                        Name = file.Name,
                        Mode = dir.Mode
                    };
                    _migrationResult.Files.Add(migrationState);

                    // Skipf if not in environment
                    if (enableEnvMatch.IsMatch(file.Name) && !envMatch.IsMatch(file.Name))
                    {
                        _logger.LogInformation($"Skipping: {file.Name} as is not in environment: {_options.DbContext}");
                        continue;
                    }

                    var hash = await _hashService.HashBase64(file.PhysicalPath);
                    var info = await _server.GetFileInfo(file.Name);

                    if (info != null)
                    {
                        // Not modified and not Always
                        if (hash == info.Hash && (dir.Mode == DataTruck.Models.ApplyModes.OneTime || dir.Mode == DataTruck.Models.ApplyModes.Updated))
                        {
                            _logger.LogInformation($"Skipping file: {file.Name} (already applied)");
                            continue;
                        }
                        // If onetime, skip but warning message
                        if (hash != info.Hash && dir.Mode == DataTruck.Models.ApplyModes.OneTime)
                        {
                            _logger.LogWarning($"Modified file: {file.Name} will not be applied again ({dir.Mode})");
                            migrationState.State = DataTruck.Models.State.IgnoredModified;
                            if (_options.ModifiedOneTimeScriptRaiseError)
                            {
                                _migrationResult.Errors.Add(new MigrationError { FileName = file.Name, Error = "File is modified but already applied to database." });
                            }
                            continue;
                        }
                    }

                    _logger.LogInformation($"Applying: {file.Name}");
                    try
                    {
                        await _server.ApplyFile(_options.ConnectionString, file.PhysicalPath, _options.CommandTimeout);
                        var result = await _server.UpsertFileInfo(file.Name, hash);
                        
                        migrationState.State = DataTruck.Models.State.Applied;
                        _logger.LogInformation($"Applied correctly: {file.Name}");
                    } catch (Exception e)
                    {
                        migrationState.State = DataTruck.Models.State.Error;
                        _logger.LogError(e, $"Error applying file: {file.Name}");
                        _migrationResult.Errors.Add(new MigrationError { FileName = file.Name, Error = $"Error applying file: {file.Name}" });
                        return _migrationResult;
                    }
                }

            }
            _logger.LogInformation("Migration completed");

            return _migrationResult;
        }
        private void GetFiles(IFileProviderEx provider, string path, List<IFileInfo> files)
        {
            var test = _fileProvider.GetDirectoryContents(path);
            foreach (var entry in test)
            {
                if (!entry.IsDirectory && entry.Name.EndsWith(".sql"))
                {
                    files.Add(entry);
                }
                GetFiles(provider, Path.Combine(path, entry.Name), files);
            }
        }

    }
}

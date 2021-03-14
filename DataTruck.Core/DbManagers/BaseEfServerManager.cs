using DataTruck.Data;
using DataTruck.Data.Models;
using DataTruck.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.DbManagers
{
    public abstract class BaseEfServerManager : IDbServer
    {
        private readonly ILogger _logger;
        private readonly DataContext _dataContext;

        public BaseEfServerManager(ILogger logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<SqlFile> GetFileInfo(string name)
        {
            return await _dataContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task<SqlFile> UpsertFileInfo(string name, string hash)
        {
            var file = await _dataContext.Files.FirstOrDefaultAsync(e => e.Name == name);

            if (file == null)
            {
                file = new SqlFile
                {
                    Name = name,
                    Created = DateTimeOffset.UtcNow,
                };
            }

            file.Updated = DateTimeOffset.UtcNow;
            file.Hash = hash;

            _dataContext.Update(file);
            await _dataContext.SaveChangesAsync();

            return file;
        }

        public virtual async Task Initialize(CancellationToken stoppingToken) =>
            await _dataContext.Database.MigrateAsync(stoppingToken);
        
        // To implement by child classes!
        public abstract Task ApplyFile(string connectionString, string file, int timeoutSeconds);
    }
}

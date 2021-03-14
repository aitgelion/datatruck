using DataTruck.Comparers;
using DataTruck.Core;
using DataTruck.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DataTruck.Test
{
    public class IntegratedRunnerInstanceTest
    {
        public IntegratedRunnerInstanceTest()
        {
            // Mock services? 
        }

        [Fact]
        public async Task CreateRunnerInstance()
        {
            var sqlFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "emptytestdir");
            if (!Directory.Exists(sqlFilesPath))
            {
                Directory.CreateDirectory(sqlFilesPath);
            }

            IRunner runner = Runner.Create(new Configuration.MigrationOptions
            {
                DbServer = "InMemory",
                ConnectionString = "MOCK",
                SqlFilesPath = sqlFilesPath
            });

            var result = await runner.Run();
            Assert.Empty(result.Errors);
        }

        public async Task<string> CreateMigrationFiles(string basePath, Dictionary<string, string> testFiles)
        {
            var sqlFilesPath = Path.Combine(Directory.GetCurrentDirectory(), basePath);
            if (!Directory.Exists(sqlFilesPath))
            {
                Directory.CreateDirectory(sqlFilesPath);
            }

            foreach (var file in testFiles)
            {
                var dirPath = Path.Combine(sqlFilesPath, "First");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                await File.WriteAllTextAsync(Path.Combine(dirPath, file.Key), file.Value);
            }

            return sqlFilesPath;
        }

        [Fact]
        public async Task SimpleExecution()
        {
            // Create files:
            Dictionary<string, string> testFiles = new()
            {
                { "0001_create_table.sql", "CREATE TABLE employees(id INT IDENTITY(1, 1) PRIMARY KEY,fullname VARCHAR(50) NOT NULL); " },
                { "0001.0001_Insert_data.sql", "INSERT INTO employees (fullname) VALUES ('Paco Jones');" },
                { "0003_Schema_update.sql", "ALTER TABLE employees ADD salary DECIMAL(19,4) DEFAULT(0.00);" },
                { "0003.0001_Insert_data.sql", "INSERT INTO employees (fullname, salary) VALUES ('Benito Camelas', 65000.00);" }
            };

            var sqlFilesPath = await CreateMigrationFiles("testfiles", testFiles);

            IRunner runner = Runner.Create(new Configuration.MigrationOptions
            {
                DbServer = "InMemory",
                ConnectionString = "MOCK1",
                SqlFilesPath = sqlFilesPath,
                Directories = new List<Models.SqlDirectoryModel>()
                {
                    new Models.SqlDirectoryModel{ Name = "First", Order = 0, Mode = Models.ApplyModes.OneTime }
                }
            });

            var result = await runner.Run();
            Assert.Empty(result.Errors);
            foreach (var name in testFiles.Keys)
            {
                var state = result.Files.First(f => f.Name == name);
                Assert.Equal(Models.State.Applied, state.State);
            }
            // Assert.Equal(testFiles.Keys.Select(k => new MigrationState { Name = k, Mode = Models.ApplyModes.OneTime, State = Models.State.Applied }), result.Files);
        }

        [Fact]
        public async Task ModifyOneTimeAfterExecution()
        {
            // Create files:
            Dictionary<string, string> testFiles = new()
            {
                { "0001_create_table.sql", "CREATE TABLE employees(id INT IDENTITY(1, 1) PRIMARY KEY,fullname VARCHAR(50) NOT NULL); " },
                { "0001.0001_Insert_data.sql", "INSERT INTO employees (fullname) VALUES ('Paco Jones');" },
                { "0003_Schema_update.sql", "ALTER TABLE employees ADD salary DECIMAL(19,4) DEFAULT(0.00);" },
                { "0003.0001_Insert_data.sql", "INSERT INTO employees (fullname, salary) VALUES ('Benito Camelas', 65000.00);" }
            };

            var sqlFilesPath = await CreateMigrationFiles("testfiles2", testFiles);

            IRunner runner = Runner.Create(new Configuration.MigrationOptions
            {
                DbServer = "InMemory",
                ConnectionString = "MOCK2",
                SqlFilesPath = sqlFilesPath,
                ModifiedOneTimeScriptRaiseError = false,
                Directories = new List<Models.SqlDirectoryModel>()
                {
                    new Models.SqlDirectoryModel{ Name = "First", Order = 0, Mode = Models.ApplyModes.OneTime }
                }
            });

            var result = await runner.Run();
            Assert.Empty(result.Errors);
            Assert.Equal(4, result.Files.Count);
            foreach (var name in testFiles.Keys)
            {
                var state = result.Files.First(f => f.Name == name);
                Assert.Equal(Models.State.Applied, state.State);
            }

            // Modify one file
            testFiles["0001.0001_Insert_data.sql"] = "EDITED";
            await CreateMigrationFiles("testfiles2", testFiles);

            result = await runner.Run();
            Assert.Equal(4, result.Files.Count);

            var states = new Models.State[] { Models.State.Skipped, Models.State.IgnoredModified, Models.State.Skipped, Models.State.Skipped };
            for (int i=0; i< states.Length; i++)
            {
                Assert.Equal(states[i], result.Files[i].State);
            }
        }



    }
}

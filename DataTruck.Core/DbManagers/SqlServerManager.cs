using DataTruck.Core.Services;
using DataTruck.Data;
using DataTruck.Data.Models;
using DataTruck.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.DbManagers
{
    public class SqlServerManager : BaseEfServerManager, IDbServer
    {
        private readonly ILogger<SqlServerManager> _logger;

        public SqlServerManager(ILogger<SqlServerManager> logger, DataContext dataContext): base(logger, dataContext)
        {
            _logger = logger;
        }

        public override async Task ApplyFile(string connectionString, string file, int timeoutSeconds)
        {
            // Open and close connection by applied file. Reuse?
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sql = await File.ReadAllTextAsync(file);
                if (string.IsNullOrWhiteSpace(sql))
                {
                    _logger.LogInformation($"Skipping empry file");
                    return;
                }

                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    SqlCommand command = new SqlCommand(sql, connection, (SqlTransaction)transaction);
                    command.CommandTimeout = timeoutSeconds;

                    var rows = await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                    if (rows > 0)
                    {
                        _logger.LogInformation($"Affected {rows} rows.");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Execution failed, rolling back transaction");
                    try
                    {
                        await transaction.RollbackAsync();
                    } catch (Exception e2)
                    {
                        _logger.LogError(e2, $"Error, rolling back transaction");
                    }

                    throw;
                }
            }
        }

    }
}

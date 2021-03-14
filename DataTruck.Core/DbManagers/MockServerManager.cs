using DataTruck.Core.Services;
using DataTruck.Data;
using DataTruck.Data.Models;
using DataTruck.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.DbManagers
{
    public class MockServerManager : BaseEfServerManager, IDbServer
    {
        private readonly ILogger<MockServerManager> _logger;

        public MockServerManager(ILogger<MockServerManager> logger, DataContext dataContext): base (logger, dataContext)
        {
            _logger = logger;
        }

        public override Task Initialize(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Mock Initialize");
            return Task.CompletedTask;
        }

        public override Task ApplyFile(string connectionString, string file, int timeoutSeconds)
        {
            _logger.LogInformation($"Mock file apply: {file}");
            return Task.CompletedTask;
        }

    }
}

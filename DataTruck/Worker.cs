using DataTruck.Comparers;
using DataTruck.Configuration;
using DataTruck.Core;
using DataTruck.Interfaces;
using DataTruck.Services;
using Microsoft.Extensions.DependencyInjection;
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

namespace DataTruck
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            using (var scope = _serviceProvider.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IRunner>().Run(stoppingToken);
            }
            _hostApplicationLifetime.StopApplication();
        }
    }
}

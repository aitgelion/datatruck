using DataTruck.Configuration;
using DataTruck.Core.Models;
using DataTruck.Data;
using DataTruck.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataTruck
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var result = host.Services.GetService<MigrationResult>();
            host.Run();

            if (result != null && result.EnableExitCodeError && result.Errors.Any())
            {
                return 1;
            }

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    // Allow a third party to override the default configuration, as Directories for example
                    var configuration = configurationBuilder.Build();
                    var extraOptions = configuration["ExtraOptionsFilePath"];
                    configurationBuilder.AddJsonFile(extraOptions, true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Configuration from ENV, COMMANDLINE AND APPSETTINGS
                    services.Configure<MigrationOptions>(hostContext.Configuration);

                    // Default services:
                    services.AddDefaultBehaviour(hostContext.Configuration);

                    // Setup the service
                    services.AddHostedService<Worker>();
                });
    }
}

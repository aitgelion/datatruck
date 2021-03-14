using DataTruck.Core.Models;
using DataTruck.Extensions;
using DataTruck.Interfaces;
using DataTruck.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataTruck.Extensions
{
    public static class DefaultBehaviourExtension
    {
        public static IServiceCollection AddDefaultBehaviour(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMigrationRunner()
                .AddFileNameComparer(configuration)
                .AddHashService()
                .AddDbServer(configuration)
                .AddFileProvider(configuration);

            return services;
        }
    }
}

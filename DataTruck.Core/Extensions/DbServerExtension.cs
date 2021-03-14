using DataTruck.Configuration;
using DataTruck.Data;
using DataTruck.DbManagers;
using DataTruck.Interfaces;
using DataTruck.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Extensions
{
    public static class DbServerExtension
    {
        public static IServiceCollection AddDbServer(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.Get<MigrationOptions>();
            _ = options.DbServer switch
            {
                DataContextOptionsBuilder.SqlServer => services.AddScoped<IDbServer, SqlServerManager>(),
                DataContextOptionsBuilder.Oracle    => services.AddScoped<IDbServer, OracleManager>(),
                DataContextOptionsBuilder.InMemory  => services.AddScoped<IDbServer, MockServerManager>(),
                _ => throw new Exception($"No DB provider for {configuration["DbServer"]}")
            };

            services.AddDbContext<DataContext>(options =>
            {
                var opts = configuration.Get<MigrationOptions>();
                DataContextOptionsBuilder.Configure(options, opts.DbServer, opts.ConnectionString);
            });

            return services;
        }
    }
}

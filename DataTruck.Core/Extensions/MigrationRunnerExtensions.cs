using DataTruck.Core;
using DataTruck.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DataTruck.Extensions
{
    public static class MigrationRunnerExtensions
    {
        public static IServiceCollection AddMigrationRunner(this IServiceCollection services)
        {
            services.AddSingleton<MigrationResult>();
            services.AddScoped<IRunner, Runner>();
            return services;
        }
    }

}

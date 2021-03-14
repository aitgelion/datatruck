using DataTruck.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataTruck.Extensions
{
    public static class HashExtensions
    {
        public static IServiceCollection AddHashService(this IServiceCollection services)
        {
            services.AddSingleton<IHashService, HashService>();
            return services;
        }
    }

}

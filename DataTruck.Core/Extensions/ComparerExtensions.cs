using DataTruck.Comparers;
using DataTruck.Data;
using DataTruck.Interfaces;
using DataTruck.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataTruck.Extensions
{
    public static class ComparerExtensions
    {
        public static IServiceCollection AddFileNameComparer(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: multiple file comparers based on config value?
            services.TryAddSingleton<IFileNameComparer, CustomFileNameComparer>();

            return services;
        }
    }
}

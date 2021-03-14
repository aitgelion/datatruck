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
    public static class FileProviderExtension
    {
        public static IServiceCollection AddFileProvider(this IServiceCollection services, IConfiguration configuration)
        {
            string path = configuration["SqlFilesPath"] ?? "./";

            if (path.StartsWith("."))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
                path = Path.GetFullPath(path);
            }

            services.AddSingleton<IFileProviderEx>(new PhysicalFileProviderEx(path));

            return services;
        }
        

    }
}

using DataTruck.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Core
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            var provider = args[0];
            DataContextOptionsBuilder.Configure(optionsBuilder, provider, args.Length == 2 ? args[1] : null);
            
            return new DataContext(optionsBuilder.Options);
        }
    }
}

using DataTruck.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Data
{
    public class DataContext : DbContext
    {
        public const string DefaultSchema = @"datatruck";

        // Tables
        public DbSet<SqlFile> Files { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }
    }

    public class DataContextOptionsBuilder
    {
        public const string InMemory = nameof(InMemory);
        public const string SqlServer = nameof(SqlServer);
        public const string Oracle = nameof(Oracle);

        public static void Configure(DbContextOptionsBuilder optionsBuilder, string provider, string connectionString = null)
        {
            _ = provider switch
            {
                // Initially you have to create the migration here an then move to the new project
                "initial_SqlServer" => optionsBuilder.UseSqlServer("FAKE", x =>
                {
                    x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DataContext.DefaultSchema);
                }),
                "initial_Oracle" => optionsBuilder.UseOracle("FAKE", x =>
                {
                    x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DataContext.DefaultSchema);
                }),

                // From here, use the others
                SqlServer => optionsBuilder.UseSqlServer(connectionString,
                x =>
                {
                    x.MigrationsAssembly("DataTruck.Data.Migrations.SqlServer");
                    x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DataContext.DefaultSchema);
                }),
                Oracle => optionsBuilder.UseOracle(connectionString,
                x =>
                {
                    x.MigrationsAssembly("DataTruck.Data.Migrations.Oracle");
                    x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DataContext.DefaultSchema);
                }),
                // Mock and error...
                InMemory => optionsBuilder.UseInMemoryDatabase(connectionString),
                _ => throw new Exception($"Unsupported provider: {provider}")
            };

        }
    }

}

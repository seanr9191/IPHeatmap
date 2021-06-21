using IPHeatmap.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPHeatmap.Data
{
    public class IPContext : DbContext
    {

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<IPAddress> IPAddresses { get; set; }

        public IPContext()
        {

        }

        public IPContext(DbContextOptions<IPContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                optionsBuilder.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
            }

            
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IPAddress>()
                .ToTable("IPAddress");

            /* 
             * Due to limitations with Sqlite not supporting decimal values, we must convert these to double.
             * This will allow us to do our bounding box comparison within linq/queries 
             */
            modelBuilder.Entity<IPAddress>()
                .Property(e => e.Latitude).HasConversion<double>();
            modelBuilder.Entity<IPAddress>()
                .Property(e => e.Longitude).HasConversion<double>();
        }
    }
}

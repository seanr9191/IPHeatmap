using IPHeatmap.Data;
using IPHeatmap.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace IPHeatmap.Services.Data
{
    public class SeedService
    {
        private static readonly string SeedZip = @"./Data/Seed/GeoLite2-City-CSV_20190618.zip";
        private static readonly string SeedUnzipDirectory = @"./Data/Seed/";
        private static readonly string SeedCSV = @"./Data/Seed/GeoLite2-City-Blocks-IPv4.csv";

        private readonly ILogger Logger;

        public SeedService()
        {

        }

        public SeedService(ILogger logger)
        {
            Logger = logger;
        }

        public async Task<bool> SeedDatabase()
        {
            Logger.LogInformation("Beginning Database Seeding Process...");

            IPContext context = new();

            bool csvExists = File.Exists(SeedCSV);
            bool zipExists = File.Exists(SeedZip);

            /* Neither file exists. We can't seed the database. Return false and stop the process. */
            if (!csvExists && !zipExists)
            {
                LogFailure();
                return false;
            }

            /* Zip file exists. Lets unzip it and see if we can find the CSV */
            if (!csvExists && zipExists)
            {
                ZipFile.ExtractToDirectory(SeedZip, SeedUnzipDirectory);
                csvExists = File.Exists(SeedCSV);
            }

            /* Still no CSV. We'll error and exit. */
            if (!csvExists)
            {
                LogFailure();
                return false;
            }

            /* We now have our CSV. Lets seed the database. */
            /* We skip 1 to skip the header row */
            var addresses = File.ReadAllLines(SeedCSV)
                .Skip(1)
                .Select(row => row.Split(","))
                .Select(rowItems => new IPAddress
                {
                    Network = rowItems[0],
                    GeonameId = rowItems[1].Length == 0 ? -1 : int.Parse(rowItems[1]),
                    RegisteredCountryGeonameId = rowItems[2].Length == 0 ? -1 : int.Parse(rowItems[2]),
                    RepresentedCountryGeonameId = rowItems[3].Length == 0 ? -1 : int.Parse(rowItems[3]),
                    IsAnonymousProxy = rowItems[4].Equals("1"),
                    IsSatelliteProvider = rowItems[5].Equals("1"),
                    PostalCode = rowItems[6],
                    Latitude = rowItems[7].Length == 0 ? 0 : decimal.Parse(rowItems[7]),
                    Longitude = rowItems[8].Length == 0 ? 0 : decimal.Parse(rowItems[8]),
                    AccuracyRadius = rowItems[9].Length == 0 ? -1 : int.Parse(rowItems[9])
                })
                .ToList();

            await context.IPAddresses.AddRangeAsync(addresses);
            await context.SaveChangesAsync();
            await context.DisposeAsync();
            return true;
        }

        private void LogFailure()
        {
            Logger?.LogError("Unable to seed the database due to missing data files.");
        }
    }
}

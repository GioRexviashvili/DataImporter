using Azure.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using DataImporter.DataReader;
using DataImporter.Database;

namespace DataImporter;

internal static class Program
{
    private const string ConnectionStringName = "DefaultConnection";

    static int Main(string[] args)
    {
        try
        {
            IConfiguration config = BuildConfiguration();

            string connectionString = GetConnectionString(config);
            (string filePath, int batchSize) = GetImportSettings(config);
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static string GetConnectionString(IConfiguration config)
    {
        string? cs = config.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found.");
        return cs;
    }

    private static (string FilePath, int BatchSize) GetImportSettings(IConfiguration config)
    {
        string filePath = config["ImportSettings:FilePath"]
                          ?? throw new InvalidOperationException(
                              "Import:FilePath not found in config and not provided in args.");

        if (!int.TryParse(config["Import:BatchSize"], out var batchSize) || batchSize <= 0)
            batchSize = 10000; // default

        return (filePath, batchSize);
    }
}
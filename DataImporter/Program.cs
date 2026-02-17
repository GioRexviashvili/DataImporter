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

            Guid batchId = Guid.NewGuid();

            Console.WriteLine($"BatchId: {batchId}");
            Console.WriteLine($"File: {filePath}");
            Console.WriteLine($"Batch size: {batchSize}");
            Console.WriteLine();

            using CsvDataReader reader = new CsvDataReader(new FileInfo(filePath));
            DataInserter inserter = new();

            int totalInserted = 0;
            int batchNumber = 0;

            using SqlConnection connection = new(connectionString);
            connection.Open();
            Console.WriteLine("Connection opened.\n");

            foreach (var batch in reader.GetData(batchId, batchSize))
            {
                batchNumber++;
                inserter.InsertBatch(connection, batch);
                totalInserted += batch.Count;

                if (batchNumber % 10 == 0)
                    Console.WriteLine($"Inserted {totalInserted} rows...\n");
            }

            Console.WriteLine($"Done. Inserted total: {totalInserted}\n");

            if (reader.Errors.Count > 0)
            {
                Console.WriteLine($"Total Errors: {reader.Errors.Count}\n");
                PrintErrors(reader);
                return 2;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Import failed:");
            Console.WriteLine(e);
            return 1;
        }

        return 0;
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
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
                              "ImportSettings:FilePath not found in config.");

        if (!int.TryParse(config["ImportSettings:BatchSize"], out var batchSize) || batchSize <= 0)
            batchSize = 10000; // default

        return (filePath, batchSize);
    }

    private static void PrintErrors(CsvDataReader reader)
    {
        foreach (var error in reader.Errors)
        {
            Console.WriteLine($"Line {error.LineNumber}: {error.Reason}\n");
        }
    }
}
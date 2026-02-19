using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using DataImporter.DataReader;
using DataImporter.Database;

namespace DataImporter;

internal static class Program
{
    private const string ConnectionStringName = "DefaultConnection";
    private static Guid _batchId = Guid.Empty;
    private static SqlConnection _connection;

    static int Main(string[] args)
    {
        try
        {
            IConfiguration config = BuildConfiguration();

            string connectionString = GetConnectionString(config);
            (string filePath, int batchSize) = GetImportSettings(config);

            _batchId = Guid.NewGuid();

            Console.WriteLine("\n----------------------------------------------");
            Thread.Sleep(500);
            Console.WriteLine($"BatchId: {_batchId}");
            Thread.Sleep(500);
            Console.WriteLine($"File: {filePath}");
            Thread.Sleep(500);
            Console.WriteLine($"Batch size: {batchSize:n0}");
            Thread.Sleep(500);
            Console.WriteLine("----------------------------------------------\n");
            
            using CsvDataReader reader = new CsvDataReader(new FileInfo(filePath));
            DataInserter inserter = new();

            int totalInserted = 0;
            int batchNumber = 0;

            SqlConnection connection = new(connectionString);
            _connection = connection;
            connection.Open();
            Thread.Sleep(500);
            Console.WriteLine("[ Connection opened successfully! ]\n");
            Thread.Sleep(500);

            var sw = Stopwatch.StartNew();
            Console.WriteLine("[ Start reading and inserting batches into staging table! ]\n");

            foreach (var batch in reader.GetData(_batchId, batchSize))
            {
                batchNumber++;
                inserter.InsertBatch(connection, batch);
                totalInserted += batch.Count;

                if (batchNumber % 10 == 0)
                {
                    Console.Write($"{totalInserted:n0}k rows --> ");
                }
            }
            Console.Write("Done\n");

            sw.Stop();
            
            Thread.Sleep(1000);

            Console.WriteLine($"\nInserted total: {totalInserted:n0}\n");
            Thread.Sleep(1000);
            Console.WriteLine($"Time needed: {sw.Elapsed.Seconds} seconds.\n");
            Thread.Sleep(1000);

            if (reader.Errors.Count > 0)
            {
                Console.WriteLine($"Total Errors during reading from the file: {reader.Errors.Count}\n");
                Thread.Sleep(1000);
                Console.WriteLine("Errors:");
                PrintErrors(reader);
                Console.WriteLine("\n----------------------------------------------");
                Thread.Sleep(1000);
            }

            Console.WriteLine("\nValidation & Inserting & Updating...\n");

            using (SqlCommand cmd = new SqlCommand("ProcessBatch_sp", connection))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BatchId", System.Data.SqlDbType.UniqueIdentifier)
                    { Value = _batchId });

                using SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    Guid dbBatchId = r.GetGuid(r.GetOrdinal("BatchId"));
                    int stagingRows = r.GetInt32(r.GetOrdinal("StagingRows"));
                    int validRows = r.GetInt32(r.GetOrdinal("ValidRows"));
                    int invalidRows = r.GetInt32(r.GetOrdinal("InvalidRows"));
                    int errorsCount = r.GetInt32(r.GetOrdinal("ErrorsCount"));

                    Console.WriteLine($"Database Summary:");
                    Thread.Sleep(500);
                    Console.WriteLine($"staging rows: {stagingRows:n0}");
                    Thread.Sleep(500);
                    Console.WriteLine($"valid rows:   {validRows:n0}");
                    Thread.Sleep(500);
                    Console.WriteLine($"invalid rows: {invalidRows:n0}");
                    Thread.Sleep(500);
                    Console.WriteLine($"errors count: {errorsCount:n0}\n");
                    Thread.Sleep(500);
                }
            }

            Console.WriteLine("Database validation errors:\n");

            using (SqlCommand cmd = new SqlCommand("GetBatchErrors_sp", connection))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BatchId", System.Data.SqlDbType.UniqueIdentifier)
                    { Value = _batchId });

                using SqlDataReader r = cmd.ExecuteReader();
                int printed = 0;

                while (r.Read())
                {
                    int lineNumber = r.GetInt32(r.GetOrdinal("LineNumber"));
                    string fieldName = r.GetString(r.GetOrdinal("FieldName"));
                    string rawValue = r.GetString(r.GetOrdinal("RawValue"));
                    string reason = r.GetString(r.GetOrdinal("Reason"));

                    Console.WriteLine($"line {lineNumber}: {fieldName}='{rawValue}' -> {reason}");
                    Thread.Sleep(500);
                    printed++;
                }

                if (printed == 0)
                    Console.WriteLine("no database errors \n");
            }
            Console.WriteLine("----------------------------------------------\n");
            Thread.Sleep(1000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Import failed:");
            Console.WriteLine(e);
            return 1;
        }
        finally
        {
            try
            {
                Console.WriteLine("Cleaning up database...\n");

                using SqlCommand cmd = new("CleanUpStaging_sp", _connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BatchId", SqlDbType.UniqueIdentifier) { Value = _batchId });
                cmd.ExecuteNonQuery();
                Console.WriteLine("Cleaning up Done.\n");

                _connection.Close();
                _connection.Dispose();
            }
            catch (Exception cleanupEx)
            {
                Console.WriteLine("Cleanup staging failed:");
                Console.WriteLine(cleanupEx);
            }
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
            Thread.Sleep(500);
        }
    }
}
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

    static int Main(string[] args)
    {
        try
        {
            IConfiguration config = BuildConfiguration();

            string connectionString = GetConnectionString(config);
            (string filePath, int batchSize) = GetImportSettings(config);

            _batchId = Guid.NewGuid();

            Console.WriteLine($"BatchId: {_batchId}");
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

            var sw = Stopwatch.StartNew();

            foreach (var batch in reader.GetData(_batchId, batchSize))
            {
                batchNumber++;
                inserter.InsertBatch(connection, batch);
                totalInserted += batch.Count;

                if (batchNumber % 10 == 0)
                {
                    Console.WriteLine($"Inserted {totalInserted:n0} rows...\n");
                }
            }

            sw.Stop();

            Console.WriteLine($"Done. Inserted total: {totalInserted:n0}\n");
            Console.WriteLine($"Elapsed: {sw.Elapsed}");

            Console.WriteLine("\nProcessing batch in database...\n");

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

                    Console.WriteLine($"db summary:");
                    Console.WriteLine($"batchId: {dbBatchId}");
                    Console.WriteLine($"staging rows: {stagingRows:n0}");
                    Console.WriteLine($"valid rows:   {validRows:n0}");
                    Console.WriteLine($"invalid rows: {invalidRows:n0}");
                    Console.WriteLine($"errors count: {errorsCount:n0}\n");
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
                    printed++;
                }

                if (printed == 0)
                    Console.WriteLine("no database errors \n");
            }

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
        finally
        {
            if (_batchId != Guid.Empty)
            {
                try
                {
                    IConfiguration config = BuildConfiguration();
                    string cs = GetConnectionString(config);

                    using SqlConnection cleanupConn = new(cs);
                    cleanupConn.Open();

                    using SqlCommand cmd = new("CleanUpStaging_sp", cleanupConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@BatchId", SqlDbType.UniqueIdentifier) { Value = _batchId });
                    cmd.ExecuteNonQuery();
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine("Cleanup staging failed:");
                    Console.WriteLine(cleanupEx);
                }
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
        }
    }
}
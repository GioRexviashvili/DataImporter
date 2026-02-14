using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using G12_DataImporter.DataReader;
using G12_DataImporter.DataWriter;
using G12_DataImporter.Models;
using Microsoft.Data.SqlClient;

namespace G12_DataImporter;

internal class Program
{
        static int Main(string[] args)
    {
        var fileInfo = new FileInfo(Path.Combine("DataFile", "Products.tsv"));

        try
        {
            var categories = ReadCategories(fileInfo);
            PrintCategories(categories);

            WriteToDatabase(fileInfo);

            Console.WriteLine("Data write completed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
    }

    private static IList<Category> ReadCategories(FileInfo fileInfo)
    {
        using var csvDataReader = new CsvDataReader(fileInfo);
        return csvDataReader.GetData().ToList();
    }

    private static void PrintCategories(IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            Console.WriteLine(category);
            foreach (var product in category.Products)
            {
                Console.WriteLine($"\t{product}");
            }
        }
    }

    private static void WriteToDatabase(FileInfo fileInfo)
    {
        const string connectionString = @"Server=.;Database=G12_ProductsCatalogue;Integrated Security=True;TrustServerCertificate=True";

        using var connection = new SqlConnection(connectionString);
        using var writerReader = new CsvDataReader(fileInfo);
        var writer = new SqlDataWriter(connection, writerReader);
        writer.WriteData();
    }
}
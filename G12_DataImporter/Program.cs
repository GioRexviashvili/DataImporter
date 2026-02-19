using G12_DataImporter.DataReader;
using G12_DataImporter.Models;
using G12_DataImporter.DataWriter;
using G12_DataImporter.Exceptions;
using Microsoft.Data.SqlClient;

namespace G12_DataImporter;

internal class Program
{
    static void Main(string[] args)
    {
        FileInfo fileInfo = new FileInfo(@"DataFile\Products.tsv");
        CsvDataReader csvDataReader = new CsvDataReader(fileInfo);

        SqlConnection sqlConnection = new SqlConnection("server=.;database=G12_ProductsCatalogue;integrated security=true;trustservercertificate=true;");
        SqlDataWriter sqlDataWriter = new SqlDataWriter(
            sqlConnection,
            csvDataReader,
            ex => Console.WriteLine($"Error importing data: {ex.Message}"));
        try
        {
            sqlDataWriter.WriteData();
        }
        catch (DataImportException ex)
        {
            Console.WriteLine($"Error importing data: {ex.Message}");
        }
    }
}
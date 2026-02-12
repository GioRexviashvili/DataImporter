using G12_DataImporter.DataReader;
using G12_DataImporter.Models;
using G12_DataImporter.DataWriter;

namespace G12_DataImporter;

internal class Program
{
    static void Main(string[] args)
    {
        FileInfo fileInfo = new FileInfo(@"DataFile\Products.tsv");

        CsvDataReader csvDataReader = new CsvDataReader(fileInfo);
        IEnumerable<Category> categories = csvDataReader.GetData();

        foreach (Category category in categories)
        {
            Console.WriteLine(category);
            foreach (Product product in category.Products)
            {
                Console.WriteLine($"\t{product}");
            }
        }
    }
}
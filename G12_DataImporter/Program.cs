using G12_DataImporter.DataReader;
using G12_DataImporter.Models;
using G12_DataImporter.DataWriter;

namespace G12_DataImporter;

internal class Program
{
    static void Main(string[] args)
    {
        FileInfo fileInfo = new FileInfo("/Users/irakli/Downloads/Products.txt");

        CsvDataReader csvDataReader = new CsvDataReader(fileInfo);
        IEnumerable<Category> categories = csvDataReader.GetData();
            
        foreach (var category in categories)
        {
            foreach (var product in category.Products)         
            {
                Console.WriteLine($"{category} {product}");
            }
        }

        int x = Convert.ToInt16(true);
        Console.WriteLine(x);

        const string connectionString= "Server=localhost;Database=G12_ProductsCatalogue;UID=sa;PWD=Limon4ik!;Integrated Security=False; TrustServerCertificate=True";
        SqlWriter sqlWriter = new(categories, connectionString);
        sqlWriter.WriteData();
    }
}
using G12_DataImporter.Models;

namespace G12_DataImporter.Interfaces;

public interface IDataReader
{
    IEnumerable<Category> GetData();
}
using DataImporter.Models;

namespace DataImporter.Interfaces;

public interface IDataReader
{
    IEnumerable<Category> GetData();
}

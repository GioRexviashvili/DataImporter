using DataImporter.Models;

namespace DataImporter.Interfaces;

public interface IDataReader
{
    IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize);
}

using DataImporter.Models;

namespace DataImporter.Interfaces;

public interface IDataReader
{
    IReadOnlyList<RowError> Errors { get; }
    
    IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize);
}

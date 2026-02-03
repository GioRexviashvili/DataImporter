using System.Runtime.CompilerServices;
using DataImporter.Models;

namespace DataImporter.Interfaces;

public interface IDataReader
{
    List<RowError> Errors { get; }
    
    IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize);
}

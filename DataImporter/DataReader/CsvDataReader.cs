using DataImporter.Interfaces;
using DataImporter.Models;

namespace DataImporter.DataReader;

public class CsvDataReader : IDataReader
{
    private readonly string _filePath;

    public CsvDataReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found on {filePath}");
        }

        _filePath = filePath;
    }

    public IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize)
    {
        throw new NotImplementedException();
    }
}
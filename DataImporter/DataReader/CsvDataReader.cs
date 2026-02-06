using DataImporter.Interfaces;
using DataImporter.Models;

namespace DataImporter.DataReader;

public class CsvDataReader : IDataReader
{
    private readonly string _filePath;
    public List<RowError> Errors { get; }

    public CsvDataReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found on {filePath}");
        }

        _filePath = filePath;
        Errors = new List<RowError>();
    }

    public IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize)
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size should be more than 0");

        List<ImportRow> batch = new List<ImportRow>(batchSize);

        using StreamReader streamReader = new StreamReader(_filePath);

        if (streamReader.EndOfStream)
        {
            Errors.Add(new RowError { LineNumber = 0, Reason = "File is empty" });
            yield break;
        }

        int lineNumber = 1;

        for (string? line; (line = streamReader.ReadLine()) != null; lineNumber++)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                Errors.Add(new RowError { LineNumber = lineNumber, Reason = "Empty line", RawLine = line });
                continue;
            }

            string[] values = line.Split('\t', StringSplitOptions.TrimEntries);

            if (values.Length != 7)
            {
                Errors.Add(new RowError
                {
                    LineNumber = lineNumber,
                    Reason = $"Invalid number of columns, expected 7, there was {values.Length}", RawLine = line
                });
                continue;
            }

            batch.Add(new ImportRow
            {
                BatchId = batchId,
                LineNumber = lineNumber,
                CategoryName = values[0],
                CategoryIsActiveRaw = values[1],
                ProductCode = values[2],
                ProductName = values[3],
                PriceRaw = values[4],
                QuantityRaw = values[5],
                ProductIsActiveRaw = values[6]
            });

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<ImportRow>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }
}
using DataImporter.Interfaces;
using DataImporter.Models;

namespace DataImporter.DataReader;

public sealed class CsvDataReader : IDataReader, IDisposable
{
    private readonly Stream _stream;
    private readonly bool _ownsStream;
    private readonly List<RowError> _errors = new();

    public IReadOnlyList<RowError> Errors => _errors;
    
    public CsvDataReader(FileInfo fileInfo)
    {
        ValidateFileInfo(fileInfo);

        _stream = fileInfo.OpenRead();
        _ownsStream = true;
    }

    public CsvDataReader(Stream stream)
    {
        ValidateStream(stream);
        
        _stream = stream;
        _ownsStream = false;
    }

    public IEnumerable<IReadOnlyCollection<ImportRow>> GetData(Guid batchId, int batchSize)
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be more than 0");

        _errors.Clear();

        _stream.Seek(0, SeekOrigin.Begin);
        
        List<ImportRow> batch = new List<ImportRow>(batchSize);

        using StreamReader streamReader = new StreamReader(_stream, leaveOpen: true);

        if (!streamReader.BaseStream.CanRead)
            throw new InvalidOperationException("The file is not readable.");

        if (streamReader.EndOfStream)
        {
            _errors.Add(new RowError { LineNumber = 0, Reason = "File is empty" });
            yield break;
        }

        int lineNumber = 1;

        for (string? line; (line = streamReader.ReadLine()) != null; lineNumber++)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                _errors.Add(new RowError { LineNumber = lineNumber, Reason = "Empty line", RawLine = line });
                continue;
            }

            string[] values = line.Split('\t', StringSplitOptions.TrimEntries);

            if (values.Length != 7)
            {
                _errors.Add(new RowError
                {
                    LineNumber = lineNumber,
                    Reason = $"Invalid number of columns, expected 7, there was {values.Length}", RawLine = line
                });
                continue;
            }

            CreateImportRow(batchId, batch, lineNumber, values);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<ImportRow>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    private static void ValidateFileInfo(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo, nameof(fileInfo));
        if (!fileInfo.Exists)
            throw new FileNotFoundException("The specified file was not found.", fileInfo.FullName);
        if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            throw new ArgumentException("The provided path is a directory, not a file.", nameof(fileInfo));
    }
    
    private static void ValidateStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));
        if (!stream.CanRead)
            throw new ArgumentException("The provided stream cannot be read.", nameof(stream));
        if (!stream.CanSeek)
            throw new ArgumentException("The provided stream must support seeking.", nameof(stream));
        if (stream.Position != 0)
            stream.Seek(0, SeekOrigin.Begin);
    }
    
    private static void CreateImportRow(Guid batchId, List<ImportRow> batch, int lineNumber, string[] values)
    {
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
    }

    public void Dispose()
    {
        if (_ownsStream)
            _stream.Dispose();
    }
}
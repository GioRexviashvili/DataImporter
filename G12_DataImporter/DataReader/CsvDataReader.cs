using System.Globalization;
using G12_DataImporter.Interfaces;
using G12_DataImporter.Models;

namespace G12_DataImporter.DataReader;

public sealed class CsvDataReader : IDataReader, IDisposable
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;

    public CsvDataReader(string filePath) : this(new FileInfo(filePath))
    {

    }

    public CsvDataReader(FileInfo fileInfo) 
    {
        ArgumentNullException.ThrowIfNull(fileInfo, nameof(fileInfo));
        if (!fileInfo.Exists)
            throw new FileNotFoundException("The specified file was not found.", fileInfo.FullName);
        if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            throw new ArgumentException("The provided path is a directory, not a file.", nameof(fileInfo));
        _stream = fileInfo.OpenRead();
        _leaveOpen = false;
    }

    public CsvDataReader(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));
        if (!stream.CanRead)
            throw new ArgumentException("The provided stream cannot be read.", nameof(stream));
        if (!stream.CanSeek)
            throw new ArgumentException("The provided stream must support seeking.", nameof(stream));
        if (stream.Position != 0)
            stream.Seek(0, SeekOrigin.Begin);
        _stream = stream;
        _leaveOpen = true;
    }

    public IEnumerable<Category> GetData()
    {
        using StreamReader reader = new(_stream, leaveOpen: _leaveOpen);
        if (!reader.BaseStream.CanRead)
            throw new InvalidDataException("The file cannot be read.");

        Dictionary<string, Category> categories = new();
        HashSet<string> productCodes = new();

        while (!reader.EndOfStream)
        {
            IReadOnlyList<string> tokens = GetTokens(reader);
            Category category = GetCategoryFromTokens(tokens);
            Product product = GetProductFromTokens(tokens);

            if (!productCodes.Add(product.Code))
                throw new InvalidDataException($"Product code wasn't unique : {product.Name} - {product.Code}");

            if (!categories.TryGetValue(category.Name, out Category? existingCategory))
            {
                category.Products.Add(product);
                categories.Add(category.Name, category);
            }
            else
            {
                existingCategory.Products.Add(product);
            }
        }

        return categories.Values;
    }

    private static Category GetCategoryFromTokens(IReadOnlyList<string> tokens)
        => new(tokens[0], Convert.ToBoolean(int.Parse(tokens[1])));

    private static Product GetProductFromTokens(IReadOnlyList<string> tokens)
        => new(tokens[2],
            tokens[3],
            decimal.Parse(tokens[4], CultureInfo.InvariantCulture),
            int.Parse(tokens[5]),
            Convert.ToBoolean(int.Parse(tokens[6])));

    private static string[] GetTokens(StreamReader reader)
        => reader.ReadLine()!.Split('\t');

    public void Dispose()
    {
        if (!_leaveOpen)
            _stream.Dispose();
    }
}
namespace DataImporter.Models;

public record ImportRow
{
    public Guid BatchId { get; init; }
    public int LineNumber { get; init; }

    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIsActiveRaw { get; init; } = string.Empty;

    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string PriceRaw { get; init; } = string.Empty;
    public string QuantityRaw { get; init; } = string.Empty;
    public string ProductIsActiveRaw { get; init; } = string.Empty;
}
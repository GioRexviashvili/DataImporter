namespace DataImporter.Models;

public record RowError
{
    public int LineNumber { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? RawLine { get; init; }
}
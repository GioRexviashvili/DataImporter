namespace G12_DataImporter.Exceptions;

public class DataImportException : Exception
{
    public DataImportException(string message, Exception innerException) : base(message, innerException) { }
}
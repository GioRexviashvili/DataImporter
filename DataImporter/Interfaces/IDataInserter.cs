using System.Data;
using DataImporter.Models;
using Microsoft.Data.SqlClient;

namespace DataImporter.Interfaces;

public interface IDataInserter
{
    DataTable DataTable { get; }

    public void InsertBatch(SqlConnection connection, IReadOnlyCollection<ImportRow> batch);
}
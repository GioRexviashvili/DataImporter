using System.Data;
using DataImporter.Interfaces;
using DataImporter.Models;
using Microsoft.Data.SqlClient;

namespace DataImporter.Database;

public class DataInserter : IDataInserter
{
    public DataTable DataTable { get; }

    public DataInserter()
    {
        DataTable = new DataTable();
        DataTable.Columns.Add("BatchId", typeof(Guid));
        DataTable.Columns.Add("LineNumber", typeof(int));
        DataTable.Columns.Add("CategoryName", typeof(string));
        DataTable.Columns.Add("CategoryIsActiveRaw", typeof(string));
        DataTable.Columns.Add("ProductCode", typeof(string));
        DataTable.Columns.Add("ProductName", typeof(string));
        DataTable.Columns.Add("PriceRaw", typeof(string));
        DataTable.Columns.Add("QuantityRaw", typeof(string));
        DataTable.Columns.Add("ProductIsActiveRaw", typeof(string));
    }

    public void InsertBatch(SqlConnection connection, IReadOnlyCollection<ImportRow> batch)
    {
        try
        {
            foreach (var importRow in batch)
            {
                DataTable.Rows.Add(
                    importRow.BatchId,
                    importRow.LineNumber,
                    importRow.CategoryName,
                    importRow.CategoryIsActiveRaw,
                    importRow.ProductCode,
                    importRow.ProductName,
                    importRow.PriceRaw,
                    importRow.QuantityRaw,
                    importRow.ProductIsActiveRaw
                );
            }

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "dbo.StagingTable";
            bulkCopy.WriteToServer(DataTable);
        }
        finally
        {
            DataTable.Clear();
        }
    }
}
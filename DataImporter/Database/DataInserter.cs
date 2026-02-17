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
        InitializeDataTable();
    }

    public void InsertBatch(SqlConnection connection, IReadOnlyCollection<ImportRow> batch)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));
        if (batch is null)
            throw new ArgumentNullException(nameof(batch));
        if (batch.Count == 0)
            return;
        
        if (connection.State != ConnectionState.Open)
            throw new InvalidOperationException("SqlConnection must be open.");

        try
        {
            DataTable.BeginLoadData();
            foreach (var importRow in batch)
                AddImportRowToDataTable(importRow);
            DataTable.EndLoadData();

            using SqlBulkCopy bulkCopy = new(connection, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "dbo.StagingTable",
                BulkCopyTimeout = 60,
                BatchSize = DataTable.Rows.Count,
            };

            AddColumnMappings(bulkCopy);
            bulkCopy.WriteToServer(DataTable);
        }
        finally
        {
            DataTable.Clear();
        }
    }

    private static void AddColumnMappings(SqlBulkCopy bulkCopy)
    {
        bulkCopy.ColumnMappings.Add("BatchId", "BatchId");
        bulkCopy.ColumnMappings.Add("LineNumber", "LineNumber");
        bulkCopy.ColumnMappings.Add("CategoryName", "CategoryName");
        bulkCopy.ColumnMappings.Add("CategoryIsActiveRaw", "CategoryIsActiveRaw");
        bulkCopy.ColumnMappings.Add("ProductCode", "ProductCode");
        bulkCopy.ColumnMappings.Add("ProductName", "ProductName");
        bulkCopy.ColumnMappings.Add("PriceRaw", "PriceRaw");
        bulkCopy.ColumnMappings.Add("QuantityRaw", "QuantityRaw");
        bulkCopy.ColumnMappings.Add("ProductIsActiveRaw", "ProductIsActiveRaw");
    }

    private void AddImportRowToDataTable(ImportRow importRow)
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

    private void InitializeDataTable()
    {
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
}
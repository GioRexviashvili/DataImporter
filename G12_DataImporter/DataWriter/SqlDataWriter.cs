using G12_DataImporter.Exceptions;
using G12_DataImporter.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace G12_DataImporter.DataWriter;

public class SqlDataWriter : Interfaces.IDataWriter
{
    private readonly SqlConnection _connection;
    private readonly Interfaces.IDataReader _dataReader;
    private readonly Action<DataImportException>? _logAction;
    private bool _leaveOpen = true;

    public SqlDataWriter(SqlConnection connection, Interfaces.IDataReader dataReader, Action<DataImportException> logAction = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        _logAction = logAction;
    }

    public void WriteData()
    {
        using SqlCommand command = _connection.CreateCommand();
        command.CommandText = "InsertProduct_sp";
        command.CommandType = CommandType.StoredProcedure;
        SetupParameters(command);

        IEnumerable<Category> categories = _dataReader.GetData();
        try
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                _leaveOpen = false;
            }
            foreach (var category in categories)
            {
                foreach (var product in category.Products)
                {
                    ProcessData(command, category, product);
                }
            }
        }
        finally
        {
            if (!_leaveOpen && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }

    private void ProcessData(SqlCommand command, Category category, Product product)
    {
        AssignParameters(command, category, product);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // todo: if _logAction is null, collect exceptions and throw an aggregate exception at the end of processing
            _logAction?.Invoke(new DataImportException("Error occurred while processing data.", ex));
        }
    }

    private static void AssignParameters(SqlCommand command, Category category, Product product)
    {
        command.Parameters["@CategoryName"].Value = category.Name;
        command.Parameters["@CategoryIsDeleted"].Value = !category.IsActive;
        command.Parameters["@ProductCode"].Value = product.Code;
        command.Parameters["@ProductName"].Value = product.Name;
        command.Parameters["@ProductPrice"].Value = product.Price;
        command.Parameters["@ProductQuantity"].Value = product.Quantity;
        command.Parameters["@ProductIsDeleted"].Value = !product.IsActive;
    }

    private static void SetupParameters(SqlCommand command)
    {
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar);
        command.Parameters.Add("@CategoryIsDeleted", SqlDbType.Bit);
        command.Parameters.Add("@ProductCode", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductName", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductPrice", SqlDbType.Decimal);
        command.Parameters.Add("@ProductQuantity", SqlDbType.Int);
        command.Parameters.Add("@ProductIsDeleted", SqlDbType.Bit);
    }
}
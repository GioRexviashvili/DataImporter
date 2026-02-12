using System.Data;
using G12_DataImporter.Models;
using Microsoft.Data.SqlClient;

namespace G12_DataImporter.DataWriter;

public class SqlWriter
{
    public SqlWriter(IEnumerable<Category> categories, string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        _categories = categories;
        _connectionString = connectionString;
    }

    private readonly string _connectionString;
    private readonly IEnumerable<Category> _categories;

    public void WriteData()
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand command = connection.CreateCommand();
        command.CommandText = "InsertProduct_sp";
        command.CommandType = CommandType.StoredProcedure;
        connection.Open();
        foreach (var category in _categories)
        {
            byte categoryIsDeleted = category.IsActive ? (byte)0 : (byte)1;
            foreach (var product in category.Products)
            {
                byte productIsDeleted = category.IsActive ? (byte)0 : (byte)1;
                command.Parameters.AddWithValue("@CategoryName", category.Name);
                command.Parameters.AddWithValue("@CategoryIsDeleted", categoryIsDeleted);
                command.Parameters.AddWithValue("@ProductCode", product.Code);
                command.Parameters.AddWithValue("@ProductName", product.Name);
                command.Parameters.AddWithValue("@ProductPrice", product.Price);
                command.Parameters.AddWithValue("@ProductQuantity", product.Quantity);
                command.Parameters.AddWithValue("@ProductIsDeleted", productIsDeleted);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
        }
        connection.Close();
    }
}
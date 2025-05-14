using System.Data;
using Tutorial9.Services;
using Microsoft.Data.SqlClient;
using Tutorial9.Modeks;

namespace Tutorial9.Services;

public class wareHouseService : IwareHouseService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
    
    
    public async Task<int> add(WarehouseRequest warehouse)
    {
        
        
        
        
        return 1;
    }

    public async Task ProcedureAsync(int IdProduct, int IdWarehouse, decimal Amount, DateTime CreatedAt)
    {
        await using SqlConnection con = new SqlConnection(_connectionString);
        await using SqlCommand com = new SqlCommand();
        
        com.Connection = con;
        con.OpenAsync();
        
        com.CommandText = "AddProductToWarehouse";
        com.CommandType = CommandType.StoredProcedure;
        com.Parameters.AddWithValue("@IdProduct", IdProduct);
        com.Parameters.AddWithValue("@Amount", Amount);
        com.Parameters.AddWithValue("@CreatedAt", CreatedAt);
        com.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);
        
        await com.ExecuteNonQueryAsync();
    }
}
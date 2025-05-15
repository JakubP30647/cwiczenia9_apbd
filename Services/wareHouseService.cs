using System.Data;
using Tutorial9.Services;
using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;
using Tutorial9.Modeks;

namespace Tutorial9.Services;

public class wareHouseService : IwareHouseService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cw9;Integrated Security=True;";


    public async Task<int> add(WarehouseRequest warehouse)
    {
        string command = "";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            string checkProduct = "SELECT count(*) FROM Product WHERE IdProduct = @IdProduct";
            cmd.CommandText = checkProduct;
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            int exist = (int)await cmd.ExecuteScalarAsync();

            if (exist == 0)
            {
                throw new NotFoundException($"produkt nie istnieje");
            }

            string checkMagazyn = "SELECT count(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            cmd.CommandText = checkMagazyn;
            cmd.Parameters.AddWithValue("@IdWarehouse", warehouse.IdWarehouse);
            int exist2 = (int)await cmd.ExecuteScalarAsync();

            if (exist2 == 0)
            {
                throw new NotFoundException($"magazyn nie istnieje");
            }

            if (warehouse.Amount <= 0)
            {
                throw new ConflictException("");
            }

            cmd.Parameters.Clear();

            cmd.CommandText = @"SELECT Count(*) FROM [Order] WHERE IdProduct = @IdProduct AND Amount =@Amount";
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", warehouse.Amount);

            var exist3 = (int)await cmd.ExecuteScalarAsync();

            if (exist3 == 0)
            {
                throw new NotFoundException("Not found");
            }

            cmd.Parameters.Clear();
            
            cmd.CommandText =
                @"SELECT Count(*) FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount AND CreatedAt < @CreatedAt";
            
            
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", warehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt", warehouse.CreatedAt);

            int exist4 = (int)await cmd.ExecuteScalarAsync();
            if (exist4 == 0)
            {
                throw new ConflictException("zla data");
            }
            
            cmd.Parameters.Clear();
            
            cmd.Parameters.Clear();
            string CheckOrderRealized = @"SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = (SELECT IdOrder FROM [Order] where [Order].IdProduct = @IdProduct AND [Order].Amount = @Amount)";
            cmd.CommandText = CheckOrderRealized;
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", warehouse.Amount);
            int a = (int)await cmd.ExecuteScalarAsync();
        

            if (a == 1)
            {
                throw new ConflictException("zamowienie zostalo juz zrealizowane");
            }
            
            cmd.Parameters.Clear();
        
            string aaa = @"UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = (SELECT IdOrder FROM [Order] where [Order].IdProduct = @IdProduct)";
            cmd.CommandText = aaa;
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                throw new ConflictException("");
            }
            
            cmd.Parameters.Clear();
            string ins = @"
        INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
        VALUES 
            (@IdWarehouse, @IdProduct, (SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount),
            @Amount, (SELECT Price FROM Product WHERE IdProduct = @IdProduct) * @Amount, @CreatedAt);
        
        SELECT SCOPE_IDENTITY();";
        
            cmd.CommandText = ins;
            cmd.Parameters.AddWithValue("@IdWarehouse", warehouse.IdWarehouse);
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", warehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt", warehouse.CreatedAt);
        
            var result = (int)await cmd.ExecuteNonQueryAsync();
        
            if (result == 0)
            {
                throw new Exception("dksfanfdbhshfdj");
            }
            
            
            return Convert.ToInt32(result);
        }
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
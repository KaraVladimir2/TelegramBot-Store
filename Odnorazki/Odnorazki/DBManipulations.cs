using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Telegram.Bot.Examples.Polling;

public class DBManipulations
{
    static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Orders"].ConnectionString);
    public static void Create()
    {
        
        sqlConnection.Open();
        if (sqlConnection.State == ConnectionState.Open) Console.WriteLine("DataBase opened successfully");
    }
    public static void Add()
    {        
        SqlCommand command = new SqlCommand($"INSERT INTO [Orders] (Amount, Product, Name, Delivery, Town, DeliveryAdress, Date) " +
            $"VALUES (N'{User.Amount}',N'{User.Product}',N'{User.Name}',N'{User.Delivery}',N'{User.Town}',N'{User.DeliveryAdress}',N'{DateTime.Now}')", sqlConnection);
        command.ExecuteNonQuery();
        Console.WriteLine("DataBase Add successfully");
    }  
}


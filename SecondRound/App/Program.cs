using App.Config;
using App.Data;

namespace App; 

public class Program
{
    public static async Task Main()
    {
        try
        {
            var settings = AppSet.LoadFromEnv();
            var connection = new DbConnection(settings);
            var dbInit = new DbInit(connection);

            await dbInit.InitAsync();

            Console.WriteLine("DB init es perfecto");
            Console.WriteLine("Start HTTP server: ");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Startup failed:");
            Console.WriteLine(ex.Message);
        }
    }
}
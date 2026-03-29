using System.Net.Http.Headers;
using App.Config;
using Npgsql;

namespace App.Data;

public class DbConnection(AppSet settings)
{
    private readonly string _connecitonString = settings.GetConnectionString();

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connecitonString);
    }
}
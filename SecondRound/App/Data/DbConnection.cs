using System.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;
using App.Config;
using Npgsql;

namespace App.Data;

[ExcludeFromCodeCoverage]
public class DbConnection(AppSet settings)
{
    private readonly string _connecitonString = settings.GetConnectionString();

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connecitonString);
    }
}
using Npgsql;

namespace App.Data;

public class DbInit(DbConnection connection)
{
    private readonly DbConnection _connect = connection;

    public async Task InitAsync()
    {
        await using var connection = _connect.CreateConnection();
        await connection.OpenAsync();

        var userTableSql = """
            CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                email VARCHAR(255) NOT NULL UNIQUE,
                first_name VARCHAR(100) NOT NULL,
                last_name VARCHAR(100) NOT NULL,
                password_hash TEXT NOT NULL,
                password_salt TEXT NOT NULL,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
        """;

        var sessionsTableSql = """
            CREATE TABLE IF NOT EXISTS sessions (
                id SERIAL PRIMARY KEY,
                user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                session_token TEXT NOT NULL UNIQUE,
                expires_at TIMESTAMP NOT NULL,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;

        await using var usersCommand = new NpgsqlCommand(userTableSql, connection);
        await usersCommand.ExecuteNonQueryAsync();

        await using var sessionsCommand = new NpgsqlCommand(sessionsTableSql, connection);
        await sessionsCommand.ExecuteNonQueryAsync();
    }
}
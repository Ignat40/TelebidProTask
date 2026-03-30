using System.Diagnostics.CodeAnalysis;
using App.Models;
using Npgsql;

namespace App.Data;

[ExcludeFromCodeCoverage]
public class SessionRepository : ISeshRepo
{
    private readonly DbConnection _connection;

    public SessionRepository(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task CreateAsync(Session session)
    {
        const string sql = """
            INSERT INTO sessions (user_id, session_token, expires_at)
            VALUES (@userId, @sessionToken, @expiresAt);
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("userId", session.UserId);
        command.Parameters.AddWithValue("sessionToken", session.SessionToken);
        command.Parameters.AddWithValue("expiresAt", session.ExpiresAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<Session?> GetByTokenAsync(string token)
    {
        const string sql = """
            SELECT id, user_id, session_token, expires_at, created_at
            FROM sessions
            WHERE session_token = @token;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("token", token);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Session
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            SessionToken = reader.GetString(2),
            ExpiresAt = reader.GetDateTime(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }

    public async Task DeleteByTokenAsync(string token)
    {
        const string sql = """
            DELETE FROM sessions
            WHERE session_token = @token;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("token", token);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteExpiredAsync()
    {
        const string sql = """
            DELETE FROM sessions
            WHERE expires_at < CURRENT_TIMESTAMP;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
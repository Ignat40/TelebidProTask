using App.Models;
using Npgsql;

namespace App.Data;

public class UserRepo : IUserRepo
{
    private readonly DbConnection _connection;

    public UserRepo(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT id, email, first_name, last_name, password_hash, password_salt, created_at
            FROM users
            WHERE email = @email;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapUser(reader);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT id, email, first_name, last_name, password_hash, password_salt, created_at
            FROM users
            WHERE id = @id;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapUser(reader);
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = """
            INSERT INTO users (email, first_name, last_name, password_hash, password_salt)
            VALUES (@email, @firstName, @lastName, @passwordHash, @passwordSalt)
            RETURNING id;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("firstName", user.FirstName);
        command.Parameters.AddWithValue("lastName", user.LastName);
        command.Parameters.AddWithValue("passwordHash", user.PassHash);
        command.Parameters.AddWithValue("passwordSalt", user.PassSalt);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateNameAsync(int id, string firstName, string lastName)
    {
        const string sql = """
            UPDATE users
            SET first_name = @firstName,
                last_name = @lastName
            WHERE id = @id;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("firstName", firstName);
        command.Parameters.AddWithValue("lastName", lastName);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdatePasswordAsync(int id, string passwordHash, string passwordSalt)
    {
        const string sql = """
            UPDATE users
            SET password_hash = @passwordHash,
                password_salt = @passwordSalt
            WHERE id = @id;
            """;

        await using var connection = _connection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("passwordHash", passwordHash);
        command.Parameters.AddWithValue("passwordSalt", passwordSalt);

        await command.ExecuteNonQueryAsync();
    }

    private static User MapUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(0),
            Email = reader.GetString(1),
            FirstName = reader.GetString(2),
            LastName = reader.GetString(3),
            PassHash = reader.GetString(4),
            PassSalt = reader.GetString(5),
            CreatedAt = reader.GetDateTime(6)
        };
    }
}
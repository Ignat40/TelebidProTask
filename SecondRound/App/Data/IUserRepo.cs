using App.Models;

namespace App.Data;

public interface IUserRepo
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task UpdateNameAsync(int id, string firstName, string lastName);
    Task UpdatePasswordAsync(int id, string passwordHash, string passwordSalt);
}


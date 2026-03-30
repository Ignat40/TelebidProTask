using App.Data;
using App.Models;

namespace Tests.Fakes;

public class FakeUserRepo : IUserRepo
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<int> CreateAsync(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return Task.FromResult(user.Id);
    }

    public Task UpdateNameAsync(int id, string firstName, string lastName)
    {
        var user = _users.First(u => u.Id == id);
        user.FirstName = firstName;
        user.LastName = lastName;
        return Task.CompletedTask;
    }

    public Task UpdatePasswordAsync(int id, string passwordHash, string passwordSalt)
    {
        var user = _users.First(u => u.Id == id);
        user.PassHash = passwordHash;
        user.PassSalt = passwordSalt;
        return Task.CompletedTask;
    }

    public void Seed(User user)
    {
        if (user.Id == 0)
        {
            user.Id = _nextId++;
        }

        _users.Add(user);
    }
}
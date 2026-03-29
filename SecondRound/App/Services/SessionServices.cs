using System.Security.Cryptography;
using App.Data;
using App.Models;

namespace App.Services;

public class SessionServices(ISeshRepo seshRepo, IUserRepo userRepo)
{
    private readonly ISeshRepo _seshRepo = seshRepo;
    private readonly IUserRepo _userRepo = userRepo;

    public async Task<string> CreateSessionAsync(int userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var session = new Session
        {
            UserId = userId,
            SessionToken = token,
            ExpiresAt = DateTime.UtcNow.AddHours(12)
        };

        await _seshRepo.CreateAsync(session);
        return token;
    }

    public async Task<User?> GetUserBySessionTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var session = await _seshRepo.GetByTokenAsync(token);

        if (session is null)
        {
            return null;
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            await _seshRepo.DeleteByTokenAsync(token);
            return null;
        }

        return await _userRepo.GetByIdAsync(session.UserId);
    }

    public async Task LogoutAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        await _seshRepo.DeleteByTokenAsync(token);
    }
}
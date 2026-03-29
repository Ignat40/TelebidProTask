using App.Models;
using App.Services;
using Tests.Fakes;

namespace Tests;

public class SessionServiceTests
{
    [Fact]
    public async Task CreateSessionAsync_ReturnsToken()
    {
        var sessionRepo = new FakeSessionRepository();
        var userRepo = new FakeUserRepository();
        var service = new SessionServices(sessionRepo, userRepo);

        var token = await service.CreateSessionAsync(1);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task GetUserBySessionTokenAsync_ReturnsUser_ForValidSession()
    {
        var sessionRepo = new FakeSessionRepository();
        var userRepo = new FakeUserRepository();

        userRepo.Seed(new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Ignat",
            LastName = "Petrov",
            PassHash = "hash",
            PassSalt = "salt"
        });

        await sessionRepo.CreateAsync(new Session
        {
            UserId = 1,
            SessionToken = "token123",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });

        var service = new SessionServices(sessionRepo, userRepo);
        var user = await service.GetUserBySessionTokenAsync("token123");

        Assert.NotNull(user);
        Assert.Equal("test@example.com", user!.Email);
    }

    [Fact]
    public async Task GetUserBySessionTokenAsync_ReturnsNull_ForExpiredSession()
    {
        var sessionRepo = new FakeSessionRepository();
        var userRepo = new FakeUserRepository();

        await sessionRepo.CreateAsync(new Session
        {
            UserId = 1,
            SessionToken = "expired",
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        });

        var service = new SessionServices(sessionRepo, userRepo);
        var user = await service.GetUserBySessionTokenAsync("expired");

        Assert.Null(user);
    }
}
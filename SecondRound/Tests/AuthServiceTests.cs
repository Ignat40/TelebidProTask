using App.Models;
using App.Security;
using App.Services;
using Tests.Fakes;

namespace Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ReturnsFailure_ForInvalidCaptcha()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepository();

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var service = new AuthService(
            userRepo,
            new ValidationService(),
            new PasswordHasher(),
            new CaptchaService(),
            sessionService);

        var result = await service.RegisterAsync(
            "test@example.com",
            "Ignat",
            "Petrov",
            "StrongPass1!",
            "ABCDE",
            "XXXXX");

        Assert.False(result.Success);
        Assert.Contains("Invalid CAPTCHA.", result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFailure_ForDuplicateEmail()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepository();

        userRepo.Seed(new User
        {
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "User",
            PassHash = "hash",
            PassSalt = "salt"
        });

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var service = new AuthService(
            userRepo,
            new ValidationService(),
            new PasswordHasher(),
            new CaptchaService(),
            sessionService);

        var result = await service.RegisterAsync(
            "test@example.com",
            "Ignat",
            "Petrov",
            "StrongPass1!",
            "ABCDE",
            "ABCDE");

        Assert.False(result.Success);
        Assert.Contains("Email is already registered.", result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsSuccess_ForValidData()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepository();

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var service = new AuthService(
            userRepo,
            new ValidationService(),
            new PasswordHasher(),
            new CaptchaService(),
            sessionService);

        var result = await service.RegisterAsync(
            "test@example.com",
            "Ignat",
            "Petrov",
            "StrongPass1!",
            "ABCDE",
            "ABCDE");

        Assert.True(result.Success);
        Assert.NotNull(result.UserId);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_ForWrongPassword()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepository();
        var passwordHasher = new PasswordHasher();
        var hashed = passwordHasher.HashPassword("StrongPass1!");

        userRepo.Seed(new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Ignat",
            LastName = "Petrov",
            PassHash = hashed.Hash,
            PassSalt = hashed.Salt
        });

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var service = new AuthService(
            userRepo,
            new ValidationService(),
            passwordHasher,
            new CaptchaService(),
            sessionService);

        var result = await service.LoginAsync("test@example.com", "WrongPass1!");

        Assert.False(result.Success);
    }

    [Fact]
    public async Task LoginAsync_ReturnsSuccess_ForCorrectCredentials()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepository();
        var passwordHasher = new PasswordHasher();
        var hashed = passwordHasher.HashPassword("StrongPass1!");

        userRepo.Seed(new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Ignat",
            LastName = "Petrov",
            PassHash = hashed.Hash,
            PassSalt = hashed.Salt
        });

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var service = new AuthService(
            userRepo,
            new ValidationService(),
            passwordHasher,
            new CaptchaService(),
            sessionService);

        var result = await service.LoginAsync("test@example.com", "StrongPass1!");

        Assert.True(result.Success);
        Assert.NotNull(result.SessionToken);
    }
}
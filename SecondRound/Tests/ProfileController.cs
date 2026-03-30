using App.Controllers;
using App.Models;
using App.Security;
using App.Server;
using App.Services;
using App.Views;
using Tests.Fakes;

namespace Tests;

public class ProfileControllerTests
{
    private static ProfileController CreateController(
        FakeUserRepo userRepo,
        FakeSessionRepo sessionRepo)
    {
        var sessionService = new SessionServices(sessionRepo, userRepo);

        return new ProfileController(
            sessionService,
            userRepo,
            new ValidationService(),
            new PasswordHasher(),
            new HtmlRenderer());
    }

    [Fact]
    public async Task ShowProfileAsync_Redirects_WhenNotLoggedIn()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();
        var controller = CreateController(userRepo, sessionRepo);

        var request = new HttpRequest();

        var response = await controller.ShowProfileAsync(request);

        Assert.Equal(302, response.StatusCode);
        Assert.Equal("/login", response.Headers["Location"]);
    }

    [Fact]
    public async Task ShowProfileAsync_ReturnsProfilePage_WhenLoggedIn()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();

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

        var controller = CreateController(userRepo, sessionRepo);

        var request = new HttpRequest();
        request.Cookies["session_token"] = "token123";

        var response = await controller.ShowProfileAsync(request);

        Assert.Equal(200, response.StatusCode);
        Assert.Contains("Profile", response.Body);
        Assert.Contains("test@example.com", response.Body);
    }

    [Fact]
    public async Task ShowEditProfileAsync_Redirects_WhenNotLoggedIn()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();
        var controller = CreateController(userRepo, sessionRepo);

        var response = await controller.ShowEditProfileAsync(new HttpRequest());

        Assert.Equal(302, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfileAsync_ReturnsValidationErrors_ForBadInput()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();

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

        var controller = CreateController(userRepo, sessionRepo);

        var request = new HttpRequest();
        request.Cookies["session_token"] = "token123";
        request.Form["firstName"] = "1";
        request.Form["lastName"] = "";
        request.Form["newPassword"] = "123";

        var response = await controller.UpdateProfileAsync(request);

        Assert.Equal(400, response.StatusCode);
        Assert.Contains("Invalid first name.", response.Body);
    }

    [Fact]
    public async Task UpdateProfileAsync_UpdatesNames_WhenValid()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();

        userRepo.Seed(new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name",
            PassHash = "hash",
            PassSalt = "salt"
        });

        await sessionRepo.CreateAsync(new Session
        {
            UserId = 1,
            SessionToken = "token123",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });

        var controller = CreateController(userRepo, sessionRepo);

        var request = new HttpRequest();
        request.Cookies["session_token"] = "token123";
        request.Form["firstName"] = "New";
        request.Form["lastName"] = "Person";
        request.Form["newPassword"] = "";

        var response = await controller.UpdateProfileAsync(request);
        var updatedUser = await userRepo.GetByIdAsync(1);

        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(updatedUser);
        Assert.Equal("New", updatedUser!.FirstName);
        Assert.Equal("Person", updatedUser.LastName);
        Assert.Contains("Profile updated successfully.", response.Body);
    }
}
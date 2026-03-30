using App.Controllers;
using App.Server;
using App.Services;
using App.Views;
using Tests.Fakes;

namespace Tests;

public class AuthControllerTests
{
    [Fact]
    public void ShowRegister_ReturnsHtml()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var authService = new AuthService(
            userRepo,
            new ValidationService(),
            new App.Security.PasswordHasher(),
            new CaptchaService(),
            sessionService);

        var controller = new AuthController(authService, new CaptchaService(), new HtmlRenderer());

        var response = controller.ShowRegister();

        Assert.Equal(200, response.StatusCode);
        Assert.Contains("Register", response.Body);
    }

    [Fact]
    public async Task LoginAsync_ReturnsRedirect_WhenCredentialsAreValid()
    {
        var userRepo = new FakeUserRepo();
        var sessionRepo = new FakeSessionRepo();
        var passwordHasher = new App.Security.PasswordHasher();
        var hashed = passwordHasher.HashPassword("StrongPass1!");

        userRepo.Seed(new App.Models.User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Ignat",
            LastName = "Petrov",
            PassHash = hashed.Hash,
            PassSalt = hashed.Salt
        });

        var sessionService = new SessionServices(sessionRepo, userRepo);
        var authService = new AuthService(
            userRepo,
            new ValidationService(),
            passwordHasher,
            new CaptchaService(),
            sessionService);

        var controller = new AuthController(authService, new CaptchaService(), new HtmlRenderer());

        var request = new HttpRequest();
        request.Form["email"] = "test@example.com";
        request.Form["password"] = "StrongPass1!";

        var response = await controller.LoginAsync(request);

        Assert.Equal(302, response.StatusCode);
        Assert.Equal("/profile", response.Headers["Location"]);
        Assert.NotEmpty(response.SetCookies);
    }
}
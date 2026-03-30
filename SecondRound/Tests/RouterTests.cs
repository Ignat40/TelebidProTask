using App.Controllers;
using App.Server;
using App.Services;
using App.Views;
using Tests.Fakes;

namespace Tests;

public class RouterTests
{
    private Router CreateRouter()
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

        var html = new HtmlRenderer();

        return new Router(
            new AuthController(authService, new CaptchaService(), html),
            new ProfileController(
                sessionService,
                userRepo,
                new ValidationService(),
                new App.Security.PasswordHasher(),
                html),
            sessionService,
            html);
    }

    [Fact]
    public async Task Route_Home_Returns200()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public async Task Route_Unknown_Returns404()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/unknown"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(404, response.StatusCode);
    }

    [Fact]
    public async Task Route_RegisterGet_Returns200()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/register"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(200, response.StatusCode);
        Assert.Contains("Register", response.Body);
    }

    [Fact]
    public async Task Route_LoginGet_Returns200()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/login"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(200, response.StatusCode);
        Assert.Contains("Login", response.Body);
    }

    [Fact]
    public async Task Route_ProfileGet_Redirects_WhenNotLoggedIn()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/profile"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(302, response.StatusCode);
        Assert.Equal("/login", response.Headers["Location"]);
    }

    [Fact]
    public async Task Route_EditProfileGet_Redirects_WhenNotLoggedIn()
    {
        var router = CreateRouter();

        var request = new HttpRequest
        {
            Method = "GET",
            Path = "/profile/edit"
        };

        var response = await router.RouteAsync(request);

        Assert.Equal(302, response.StatusCode);
    }
}
using App.Controllers;
using App.Views;

namespace App.Server;

public class Router
{
    private readonly AuthController _authController;
    private readonly ProfileController _profileController;
    private readonly App.Services.SessionServices _sessionServices;
    private readonly HtmlRenderer _htmlRenderer;

    public Router(
        AuthController authController,
        ProfileController profileController,
        App.Services.SessionServices sessionServices,
        HtmlRenderer htmlRenderer)
    {
        _authController = authController;
        _profileController = profileController;
        _sessionServices = sessionServices;
        _htmlRenderer = htmlRenderer;
    }

    public async Task<HttpResponse> RouteAsync(HttpRequest request)
    {
        try
        {
            if (request.Method == "GET" && request.Path == "/")
            {
                App.Models.User? currentUser = null;

                if (request.Cookies.TryGetValue("session_token", out var token))
                {
                    currentUser = await _sessionServices.GetUserBySessionTokenAsync(token);
                }

                return HttpResponse.Html(_htmlRenderer.HomePage(currentUser));
            }

            if (request.Method == "GET" && request.Path == "/register")
                return _authController.ShowRegister();

            if (request.Method == "POST" && request.Path == "/register")
                return await _authController.RegisterAsync(request);

            if (request.Method == "GET" && request.Path == "/login")
                return _authController.ShowLogin();

            if (request.Method == "POST" && request.Path == "/login")
                return await _authController.LoginAsync(request);

            if (request.Method == "POST" && request.Path == "/logout")
                return await _authController.LogoutAsync(request, _sessionServices);

            if (request.Method == "GET" && request.Path == "/profile")
                return await _profileController.ShowProfileAsync(request);

            if (request.Method == "GET" && request.Path == "/profile/edit")
                return await _profileController.ShowEditProfileAsync(request);

            if (request.Method == "POST" && request.Path == "/profile/edit")
                return await _profileController.UpdateProfileAsync(request);

            return HttpResponse.Html(_htmlRenderer.NotFoundPage(), 404);
        }
        catch (Exception ex)
        {
            return HttpResponse.Html(
                _htmlRenderer.Layout("Error", $"<h1>500</h1><p>{System.Net.WebUtility.HtmlEncode(ex.Message)}</p>"),
                500);
        }
    }
}
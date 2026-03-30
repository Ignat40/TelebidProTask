using App.Server;
using App.Services;
using App.Views;

namespace App.Controllers;

public class AuthController
{
    private readonly AuthService _authService;
    private readonly CaptchaService _captchaService;
    private readonly HtmlRenderer _htmlRenderer;

    public AuthController(AuthService authService, CaptchaService captchaService, HtmlRenderer htmlRenderer)
    {
        _authService = authService;
        _captchaService = captchaService;
        _htmlRenderer = htmlRenderer;
    }

    public HttpResponse ShowRegister()
    {
        var captchaCode = _captchaService.GenerateCode();
        var captchaSvg = _captchaService.GenerateSvg(captchaCode);

        var response = HttpResponse.Html(_htmlRenderer.RegisterPage(captchaSvg));
        response.SetCookies.Add(CookieHelper.CreateCaptchaCookie(captchaCode));
        return response;
    }

    public async Task<HttpResponse> RegisterAsync(HttpRequest request)
    {
        request.Cookies.TryGetValue("captcha_code", out var expectedCaptcha);

        var email = request.Form.GetValueOrDefault("email", string.Empty);
        var firstName = request.Form.GetValueOrDefault("firstName", string.Empty);
        var lastName = request.Form.GetValueOrDefault("lastName", string.Empty);
        var password = request.Form.GetValueOrDefault("password", string.Empty);
        var providedCaptcha = request.Form.GetValueOrDefault("captcha", string.Empty);

        var result = await _authService.RegisterAsync(
            email,
            firstName,
            lastName,
            password,
            expectedCaptcha ?? string.Empty,
            providedCaptcha);

        if (!result.Success)
        {
            var captchaCode = _captchaService.GenerateCode();
            var captchaSvg = _captchaService.GenerateSvg(captchaCode);

            var response = HttpResponse.Html(_htmlRenderer.RegisterPage(captchaSvg, result.Errors), 400);
            response.SetCookies.Add(CookieHelper.CreateCaptchaCookie(captchaCode));
            return response;
        }

        var successResponse = HttpResponse.Redirect("/login");
        successResponse.SetCookies.Add(CookieHelper.ExpireCaptchaCookie());
        return successResponse;
    }

    public HttpResponse ShowLogin(IEnumerable<string>? errors = null)
    {
        return HttpResponse.Html(_htmlRenderer.LoginPage(errors));
    }

    public async Task<HttpResponse> LoginAsync(HttpRequest request)
    {
        var email = request.Form.GetValueOrDefault("email", string.Empty);
        var password = request.Form.GetValueOrDefault("password", string.Empty);

        var result = await _authService.LoginAsync(email, password);

        if (!result.Success || string.IsNullOrWhiteSpace(result.SessionToken))
        {
            return HttpResponse.Html(_htmlRenderer.LoginPage(result.Errors), 400);
        }

        var response = HttpResponse.Redirect("/profile");
        response.SetCookies.Add(CookieHelper.CreateSessionCookie(result.SessionToken));
        return response;
    }

    public async Task<HttpResponse> LogoutAsync(HttpRequest request, SessionServices sessionServices)
    {
        request.Cookies.TryGetValue("session_token", out var token);

        if (!string.IsNullOrWhiteSpace(token))
        {
            await sessionServices.LogoutAsync(token);
        }

        var response = HttpResponse.Redirect("/login");
        response.SetCookies.Add(CookieHelper.ExpireSessionCookie());
        return response;
    }
}
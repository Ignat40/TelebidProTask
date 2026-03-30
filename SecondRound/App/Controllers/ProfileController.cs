using App.Data;
using App.Security;
using App.Server;
using App.Services;
using App.Views;

namespace App.Controllers;

public class ProfileController
{
    private readonly SessionServices _sessionServices;
    private readonly IUserRepo _userRepo;
    private readonly ValidationService _validationService;
    private readonly PasswordHasher _passwordHasher;
    private readonly HtmlRenderer _htmlRenderer;

    public ProfileController(
        SessionServices sessionServices,
        IUserRepo userRepo,
        ValidationService validationService,
        PasswordHasher passwordHasher,
        HtmlRenderer htmlRenderer)
    {
        _sessionServices = sessionServices;
        _userRepo = userRepo;
        _validationService = validationService;
        _passwordHasher = passwordHasher;
        _htmlRenderer = htmlRenderer;
    }

    public async Task<HttpResponse> ShowProfileAsync(HttpRequest request)
    {
        var user = await GetCurrentUserAsync(request);
        if (user is null)
        {
            return HttpResponse.Redirect("/login");
        }

        return HttpResponse.Html(_htmlRenderer.ProfilePage(user));
    }

    public async Task<HttpResponse> ShowEditProfileAsync(HttpRequest request)
    {
        var user = await GetCurrentUserAsync(request);
        if (user is null)
        {
            return HttpResponse.Redirect("/login");
        }

        return HttpResponse.Html(_htmlRenderer.EditProfilePage(user));
    }

    public async Task<HttpResponse> UpdateProfileAsync(HttpRequest request)
    {
        var user = await GetCurrentUserAsync(request);
        if (user is null)
        {
            return HttpResponse.Redirect("/login");
        }

        var firstName = request.Form.GetValueOrDefault("firstName", string.Empty);
        var lastName = request.Form.GetValueOrDefault("lastName", string.Empty);
        var newPassword = request.Form.GetValueOrDefault("newPassword", string.Empty);

        var errors = new List<string>();

        if (!_validationService.IsValidName(firstName))
        {
            errors.Add("Invalid first name.");
        }

        if (!_validationService.IsValidName(lastName))
        {
            errors.Add("Invalid last name.");
        }

        if (!string.IsNullOrWhiteSpace(newPassword) && !_validationService.IsValidPassword(newPassword))
        {
            errors.Add("New password is not strong enough.");
        }

        if (errors.Any())
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            return HttpResponse.Html(_htmlRenderer.EditProfilePage(user, errors), 400);
        }

        await _userRepo.UpdateNameAsync(user.Id, firstName.Trim(), lastName.Trim());

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var (hash, salt) = _passwordHasher.HashPassword(newPassword);
            await _userRepo.UpdatePasswordAsync(user.Id, hash, salt);
        }

        var updatedUser = await _userRepo.GetByIdAsync(user.Id);
        return HttpResponse.Html(_htmlRenderer.ProfilePage(updatedUser!, "Profile updated successfully."));
    }

    private async Task<App.Models.User?> GetCurrentUserAsync(HttpRequest request)
    {
        request.Cookies.TryGetValue("session_token", out var token);
        return await _sessionServices.GetUserBySessionTokenAsync(token ?? string.Empty);
    }
}
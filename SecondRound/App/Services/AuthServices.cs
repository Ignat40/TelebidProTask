using App.Data;
using App.Models;
using App.Security;

namespace App.Services;

public class AuthService(
    IUserRepo userRepo,
    ValidationService validationService,
    PasswordHasher passwordHasher,
    CaptchaService captchaService,
    SessionServices sessionService)
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly ValidationService _validationService = validationService;
    private readonly PasswordHasher _passwordHasher = passwordHasher;
    private readonly CaptchaService _captchaService = captchaService;
    private readonly SessionServices _sessionService = sessionService;

    public async Task<ServiceResult> RegisterAsync(
        string email,
        string firstName,
        string lastName,
        string password,
        string expectedCaptcha,
        string providedCaptcha)
    {
        var validationErrors = _validationService.ValidateRegistration(email, firstName, lastName, password);

        if (validationErrors.Any())
        {
            return new ServiceResult
            {
                Success = false,
                Errors = validationErrors
            };
        }

        if (!_captchaService.Verify(expectedCaptcha, providedCaptcha))
        {
            return ServiceResult.Failure("Invalid CAPTCHA.");
        }

        var existingUser = await _userRepo.GetByEmailAsync(email.Trim());
        if (existingUser is not null)
        {
            return ServiceResult.Failure("Email is already registered.");
        }

        var (hash, salt) = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Email = email.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PassHash = hash,
            PassSalt = salt
        };

        var userId = await _userRepo.CreateAsync(user);
        return ServiceResult.Ok(userId: userId);
    }

    public async Task<ServiceResult> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult.Failure("Email and password are required.");
        }

        var user = await _userRepo.GetByEmailAsync(email.Trim());
        if (user is null)
        {
            return ServiceResult.Failure("Invalid email or password.");
        }

        var passwordValid = _passwordHasher.VerifyPassword(password, user.PassHash, user.PassSalt);
        if (!passwordValid)
        {
            return ServiceResult.Failure("Invalid email or password.");
        }

        var sessionToken = await _sessionService.CreateSessionAsync(user.Id);
        return ServiceResult.Ok(userId: user.Id, sessionToken: sessionToken);
    }
}
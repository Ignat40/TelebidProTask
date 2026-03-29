using System.Text.RegularExpressions;

namespace App.Services;

public class ValidationService
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex NameRegex =
        new(@"^[A-Za-zÀ-ÿ'\- ]{2,50}$", RegexOptions.Compiled);

    public List<string> ValidateRegistration(
        string email,
        string firstName,
        string lastName,
        string password)
    {
        var errors = new List<string>();

        if (!IsValidEmail(email))
        {
            errors.Add("Invalid email.");
        }

        if (!IsValidName(firstName))
        {
            errors.Add("Invalid first name.");
        }

        if (!IsValidName(lastName))
        {
            errors.Add("Invalid last name.");
        }

        if (!IsValidPassword(password))
        {
            errors.Add("Password must be at least 8 characters and contain uppercase, lowercase, digit, and special character.");
        }

        return errors;
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email.Trim());
    }

    public bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return NameRegex.IsMatch(name.Trim());
    }

    public bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            return false;
        }

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
using App.Services;

namespace Tests;

public class ValidationServiceTests
{
    private readonly ValidationService _validationService = new();

    [Fact]
    public void IsValidEmail_ReturnsTrue_ForValidEmail()
    {
        Assert.True(_validationService.IsValidEmail("test@example.com"));
    }

    [Fact]
    public void IsValidEmail_ReturnsFalse_ForInvalidEmail()
    {
        Assert.False(_validationService.IsValidEmail("bad-email"));
    }

    [Fact]
    public void IsValidName_ReturnsTrue_ForValidName()
    {
        Assert.True(_validationService.IsValidName("Ivan"));
    }

    [Fact]
    public void IsValidName_ReturnsFalse_ForNameWithDigits()
    {
        Assert.False(_validationService.IsValidName("Ivan123"));
    }

    [Fact]
    public void IsValidPassword_ReturnsTrue_ForStrongPassword()
    {
        Assert.True(_validationService.IsValidPassword("StrongPass1!"));
    }

    [Fact]
    public void IsValidPassword_ReturnsFalse_ForWeakPassword()
    {
        Assert.False(_validationService.IsValidPassword("weak"));
    }

    [Fact]
    public void ValidateRegistration_ReturnsNoErrors_ForValidInput()
    {
        var errors = _validationService.ValidateRegistration(
            "test@example.com",
            "Ivan",
            "Petrov",
            "StrongPass1!");

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateRegistration_ReturnsFourErrors_ForInvalidInput()
    {
        var errors = _validationService.ValidateRegistration(
            "bad-email",
            "1",
            "",
            "123");

        Assert.Equal(4, errors.Count);
    }
}
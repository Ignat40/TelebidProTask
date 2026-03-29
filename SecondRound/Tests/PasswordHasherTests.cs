using App.Security;

namespace Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void HashPassword_ReturnsHashAndSalt()
    {
        var result = _passwordHasher.HashPassword("StrongPass1!");

        Assert.False(string.IsNullOrWhiteSpace(result.Hash));
        Assert.False(string.IsNullOrWhiteSpace(result.Salt));
    }

    [Fact]
    public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
    {
        var hashed = _passwordHasher.HashPassword("StrongPass1!");

        var result = _passwordHasher.VerifyPassword(
            "StrongPass1!",
            hashed.Hash,
            hashed.Salt);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_ForWrongPassword()
    {
        var hashed = _passwordHasher.HashPassword("StrongPass1!");

        var result = _passwordHasher.VerifyPassword(
            "WrongPass1!",
            hashed.Hash,
            hashed.Salt);

        Assert.False(result);
    }

    [Fact]
    public void HashPassword_GeneratesDifferentSalt_ForSamePassword()
    {
        var first = _passwordHasher.HashPassword("StrongPass1!");
        var second = _passwordHasher.HashPassword("StrongPass1!");

        Assert.NotEqual(first.Salt, second.Salt);
        Assert.NotEqual(first.Hash, second.Hash);
    }
}
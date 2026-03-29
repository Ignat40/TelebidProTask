using App.Services;

namespace Tests;

public class CaptchaServiceTests
{
    private readonly CaptchaService _captchaService = new();

    [Fact]
    public void GenerateCode_ReturnsExpectedLength()
    {
        var code = _captchaService.GenerateCode(6);
        Assert.Equal(6, code.Length);
    }

    [Fact]
    public void Verify_ReturnsTrue_ForMatchingCodes()
    {
        Assert.True(_captchaService.Verify("ABC12", "abc12"));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForDifferentCodes()
    {
        Assert.False(_captchaService.Verify("ABC12", "ZZZ99"));
    }

    [Fact]
    public void GenerateSvg_ReturnsSvgMarkup()
    {
        var svg = _captchaService.GenerateSvg("ABC12");
        Assert.Contains("<svg", svg);
        Assert.Contains("</svg>", svg);
        Assert.Contains("A", svg);
    }
}
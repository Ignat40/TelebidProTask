using App.Server;

namespace Tests;

public class CookieHelperTests
{
    [Fact]
    public void ParseCookies_ParsesCorrectly()
    {
        var cookies = CookieHelper.ParseCookies("a=1; b=2");

        Assert.Equal("1", cookies["a"]);
        Assert.Equal("2", cookies["b"]);
    }

    [Fact]
    public void CreateSessionCookie_ContainsToken()
    {
        var cookie = CookieHelper.CreateSessionCookie("abc");

        Assert.Contains("abc", cookie);
    }

    [Fact]
    public void ExpireSessionCookie_Works()
    {
        var cookie = CookieHelper.ExpireSessionCookie();

        Assert.Contains("Max-Age=0", cookie);
    }
}
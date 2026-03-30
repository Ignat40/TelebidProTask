using App.Server;

namespace Tests;

public class HttpResponseTests
{
    [Fact]
    public void Html_ResponseContainsBody()
    {
        var response = HttpResponse.Html("<h1>Test</h1>");
        var bytes = response.ToBytes();
        var text = System.Text.Encoding.UTF8.GetString(bytes);

        Assert.Contains("200 OK", text);
        Assert.Contains("<h1>Test</h1>", text);
    }

    [Fact]
    public void Redirect_SetsLocationHeader()
    {
        var response = HttpResponse.Redirect("/login");

        Assert.Equal(302, response.StatusCode);
        Assert.Equal("/login", response.Headers["Location"]);
    }

    [Fact]
    public void ToBytes_Contains404ReasonPhrase()
    {
        var response = HttpResponse.Html("not found", 404);
        var text = System.Text.Encoding.UTF8.GetString(response.ToBytes());

        Assert.Contains("404 Not Found", text);
    }

    [Fact]
    public void ToBytes_Contains500ReasonPhrase()
    {
        var response = HttpResponse.Html("error", 500);
        var text = System.Text.Encoding.UTF8.GetString(response.ToBytes());

        Assert.Contains("500 Internal Server Error", text);
    }

    [Fact]
    public void ToBytes_IncludesSetCookieHeaders()
    {
        var response = HttpResponse.Html("ok");
        response.SetCookies.Add("session_token=abc; Path=/");

        var text = System.Text.Encoding.UTF8.GetString(response.ToBytes());

        Assert.Contains("Set-Cookie: session_token=abc; Path=/", text);
    }
}
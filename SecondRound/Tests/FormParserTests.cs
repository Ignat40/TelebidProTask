using App.Server;

namespace Tests;

public class FormParserTests
{
    [Fact]
    public void ParseFormUrlEncoded_ParsesCorrectly()
    {
        var result = FormParser.ParseFormUrlEncoded("email=test@test.com&name=Ivan");

        Assert.Equal("test@test.com", result["email"]);
        Assert.Equal("Ivan", result["name"]);
    }

    [Fact]
    public void ParseQueryString_ParsesCorrectly()
    {
        var result = FormParser.ParseQueryString("?a=1&b=2");

        Assert.Equal("1", result["a"]);
        Assert.Equal("2", result["b"]);
    }
}
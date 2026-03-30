namespace App.Server;

public static class FormParser
{
    public static Dictionary<string, string> ParseFormUrlEncoded(string body)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(body))
        {
            return result;
        }

        var pairs = body.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2);
            var key = UrlDecode(parts[0]);
            var value = parts.Length > 1 ? UrlDecode(parts[1]) : string.Empty;

            result[key] = value;
        }

        return result;
    }

    public static Dictionary<string, string> ParseQueryString(string queryString)
    {
        if (queryString.StartsWith("?"))
        {
            queryString = queryString[1..];
        }

        return ParseFormUrlEncoded(queryString);
    }

    private static string UrlDecode(string value)
    {
        return Uri.UnescapeDataString(value.Replace("+", " "));
    }
}
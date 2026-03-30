using System.Text;

namespace App.Server;

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string ContentType { get; set; } = "text/html; charset=utf-8";
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; } = new();
    public List<string> SetCookies { get; } = new();

    public byte[] ToBytes()
    {
        var bodyBytes = Encoding.UTF8.GetBytes(Body);
        var sb = new StringBuilder();

        sb.Append($"HTTP/1.1 {StatusCode} {GetReasonPhrase(StatusCode)}\r\n");
        sb.Append($"Content-Type: {ContentType}\r\n");
        sb.Append($"Content-Length: {bodyBytes.Length}\r\n");

        foreach (var header in Headers)
        {
            sb.Append($"{header.Key}: {header.Value}\r\n");
        }

        foreach (var cookie in SetCookies)
        {
            sb.Append($"Set-Cookie: {cookie}\r\n");
        }

        sb.Append("\r\n");

        var headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
        return headerBytes.Concat(bodyBytes).ToArray();
    }

    public static HttpResponse Html(string html, int statusCode = 200)
    {
        return new HttpResponse
        {
            StatusCode = statusCode,
            ContentType = "text/html; charset=utf-8",
            Body = html
        };
    }

    public static HttpResponse Redirect(string location)
    {
        var response = new HttpResponse
        {
            StatusCode = 302,
            Body = string.Empty
        };

        response.Headers["Location"] = location;
        return response;
    }

    private static string GetReasonPhrase(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            302 => "Found",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "OK"
        };
    }
}
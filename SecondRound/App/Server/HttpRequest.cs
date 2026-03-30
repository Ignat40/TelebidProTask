namespace App.Server;

public class HttpRequest
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string RawPath { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Query { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Form { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Cookies { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string Body { get; set; } = string.Empty;
}
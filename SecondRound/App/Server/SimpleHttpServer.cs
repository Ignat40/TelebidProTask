using System.Net;
using System.Net.Sockets;
using System.Text;
using App.Models;

namespace App.Server;

public class SimpleHttpServer
{
    private readonly int _port;
    private readonly Router _router;

    public SimpleHttpServer(int port, Router router)
    {
        _port = port;
        _router = router;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();

        Console.WriteLine($"Server listening on http://localhost:{_port}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationToken);
            _ = Task.Run(() => HandleClientAsync(client), cancellationToken);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);

            var requestLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(requestLine))
            {
                return;
            }

            var request = new HttpRequest();
            var requestParts = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            request.Method = requestParts[0].Trim().ToUpperInvariant();
            request.RawPath = requestParts[1].Trim();

            var pathParts = request.RawPath.Split('?', 2);
            request.Path = pathParts[0];
            if (pathParts.Length > 1)
            {
                request.Query = FormParser.ParseQueryString(pathParts[1]);
            }

            string? line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                var headerParts = line.Split(':', 2);
                if (headerParts.Length == 2)
                {
                    request.Headers[headerParts[0].Trim()] = headerParts[1].Trim();
                }
            }

            request.Cookies = CookieHelper.ParseCookies(
                request.Headers.GetValueOrDefault("Cookie"));

            if (request.Headers.TryGetValue("Content-Length", out var contentLengthValue) &&
                int.TryParse(contentLengthValue, out var contentLength) &&
                contentLength > 0)
            {
                var buffer = new char[contentLength];
                var read = 0;

                while (read < contentLength)
                {
                    var chunk = await reader.ReadAsync(buffer, read, contentLength - read);
                    if (chunk == 0)
                    {
                        break;
                    }

                    read += chunk;
                }

                request.Body = new string(buffer, 0, read);

                if (request.Headers.TryGetValue("Content-Type", out var contentType) &&
                    contentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    request.Form = FormParser.ParseFormUrlEncoded(request.Body);
                }
            }

            var response = await _router.RouteAsync(request);
            var responseBytes = response.ToBytes();

            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            await stream.FlushAsync();
        }
    }

}
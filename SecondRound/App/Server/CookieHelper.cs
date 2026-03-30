namespace App.Server;

public static class CookieHelper
{
    public static Dictionary<string, string> ParseCookies(string? cookieHeader)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(cookieHeader))
        {
            return result;
        }

        var cookies = cookieHeader.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var cookie in cookies)
        {
            var parts = cookie.Split('=', 2);
            if (parts.Length == 2)
            {
                result[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return result;
    }

    public static string CreateSessionCookie(string token)
    {
        return $"session_token={token}; Path=/; HttpOnly; SameSite=Lax";
    }

    public static string ExpireSessionCookie()
    {
        return "session_token=; Path=/; HttpOnly; SameSite=Lax; Max-Age=0";
    }

    public static string CreateCaptchaCookie(string captchaCode)
    {
        return $"captcha_code={captchaCode}; Path=/; HttpOnly; SameSite=Lax";
    }

    public static string ExpireCaptchaCookie()
    {
        return "captcha_code=; Path=/; HttpOnly; SameSite=Lax; Max-Age=0";
    }
}
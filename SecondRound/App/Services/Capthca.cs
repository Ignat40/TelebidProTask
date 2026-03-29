using System.Security.Cryptography;
using System.Text;

namespace App.Services;

public class CaptchaService
{
    private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string GenerateCode(int length = 5)
    {
        var chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = AllowedChars[RandomNumberGenerator.GetInt32(AllowedChars.Length)];
        }

        return new string(chars);
    }

    public bool Verify(string expected, string actual)
    {
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(actual))
        {
            return false;
        }

        return string.Equals(
            expected.Trim(),
            actual.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateSvg(string code)
    {
        var random = new Random();
        var sb = new StringBuilder();

        sb.AppendLine("<svg xmlns='http://www.w3.org/2000/svg' width='180' height='60'>");
        sb.AppendLine("<rect width='100%' height='100%' fill='white' />");

        for (int i = 0; i < 8; i++)
        {
            sb.AppendLine(
                $"<line x1='{random.Next(0, 180)}' y1='{random.Next(0, 60)}' x2='{random.Next(0, 180)}' y2='{random.Next(0, 60)}' stroke='gray' stroke-width='1' />");
        }

        for (int i = 0; i < code.Length; i++)
        {
            int x = 20 + i * 28;
            int y = random.Next(30, 50);
            int rotate = random.Next(-20, 20);

            sb.AppendLine(
                $"<text x='{x}' y='{y}' font-size='28' font-family='Arial' transform='rotate({rotate},{x},{y})'>{code[i]}</text>");
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }
}
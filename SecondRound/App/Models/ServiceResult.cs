namespace App.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public int? UserId { get; set; }
    public string? SessionToken { get; set; }

    public static ServiceResult Failure(params string[] errors)
    {
        return new ServiceResult
        {
            Success = false,
            Errors = errors.ToList()
        };
    }

    public static ServiceResult Ok(int? userId = null, string? sessionToken = null)
    {
        return new ServiceResult
        {
            Success = true,
            UserId = userId,
            SessionToken = sessionToken
        };
    }
}
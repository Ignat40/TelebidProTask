namespace App.Models;

public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
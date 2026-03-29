namespace App.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PassHash { get; set; } = string.Empty;
    public string PassSalt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
} 
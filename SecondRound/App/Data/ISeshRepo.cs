using App.Models;

namespace App.Data;

public interface ISeshRepo
{
    Task CreateAsync(Session session);
    Task<Session?> GetByTokenAsync(string token);
    Task DeleteByTokenAsync(string token);
    Task DeleteExpiredAsync();
}
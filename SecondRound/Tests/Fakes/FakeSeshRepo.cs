using App.Data;
using App.Models;

namespace Tests.Fakes;

public class FakeSessionRepo : ISeshRepo
{
    private readonly List<Session> _sessions = new();

    public Task CreateAsync(Session session)
    {
        session.Id = _sessions.Count + 1;
        _sessions.Add(session);
        return Task.CompletedTask;
    }

    public Task<Session?> GetByTokenAsync(string token)
    {
        var session = _sessions.FirstOrDefault(s => s.SessionToken == token);
        return Task.FromResult(session);
    }

    public Task DeleteByTokenAsync(string token)
    {
        _sessions.RemoveAll(s => s.SessionToken == token);
        return Task.CompletedTask;
    }

    public Task DeleteExpiredAsync()
    {
        _sessions.RemoveAll(s => s.ExpiresAt < DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
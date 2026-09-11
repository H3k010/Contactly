using ContactsManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ContactsManager.Infrastructure.Services.Account;

public class StepUpAuthentication : IStepUpAuthentication
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const int ExpirationMinutes = 10;

    public StepUpAuthentication(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated(string action)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        if (session == null)
            return false;

        var key = $"StepUp:{action}";

        var value = session.GetString(key);

        if (value == null)
            return false;

        if (!DateTime.TryParse(value, out var authenticatedAt))
            return false;

        if (DateTime.UtcNow - authenticatedAt >
            TimeSpan.FromMinutes(ExpirationMinutes))
        {
            session.Remove(key);
            return false;
        }

        return true;
    }

    public void Authenticate(string action)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        if (session == null)
            return;

        session.SetString(
            $"StepUp:{action}",
            DateTime.UtcNow.ToString("O"));
    }

    public void Clear(string action)
    {
        _httpContextAccessor.HttpContext?
            .Session.Remove($"StepUp:{action}");
    }
}
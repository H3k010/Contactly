using ContactsManager.Domain.ValueObjects;

namespace ContactsManager.Application.DTO;

public class UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public Preferences UserPreferences { get; init; } = new();
    public bool IsAuthenticated { get; init; }
    public bool EmailConfirmed { get; init; }
}
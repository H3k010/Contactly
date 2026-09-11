using ContactsManager.Domain.ValueObjects;

namespace ContactsManager.Application.DTO;

public class ChangePreferencesDto
{
    public Preferences UserPreferences { get; set; } = new();
}
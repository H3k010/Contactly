using ContactsManager.Application.DTO;

namespace ContactsManager.Presentation.Models;

public class ProfileSettingsViewModel
{
    public ChangeNameDto Name { get; set; } = new();
    public ChangeEmailDto Email { get; set; } = new();
    public ChangePhoneDto Phone{ get; set; } = new();
    public ChangePreferencesDto UserPreferences { get; set; } = new();
    public ConfirmPasswordViewModel ConfirmPassword { get; set; } = new();
}
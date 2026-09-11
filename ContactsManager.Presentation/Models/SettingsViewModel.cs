namespace ContactsManager.Presentation.Models;

public class SettingsViewModel
{
    public string? DisplayName { get; set; }
    public string? Email {get; set;}
    public bool IsAuthenticated { get; set; }
    public bool IsEmailConfirmed {get; set;}
    public ConfirmPasswordViewModel ConfirmPassword { get; set; } = new();
}
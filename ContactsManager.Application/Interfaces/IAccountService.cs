using ContactsManager.Application.DTO;
using ContactsManager.Domain.ValueObjects;

namespace ContactsManager.Application.Interfaces;

public interface IAccountService
{
    Task<RegisterResult> RegisterAsync(RegisterDto registerDto);
    Task<AuthenticationResultDto> LoginAsync(LoginDto loginDto);
    Task LogoutAsync();
    Task<AuthenticationResultDto> ChangeUserNameAsync(string newUserName);
    Task<AuthenticationResultDto> ConfirmEmailAsync(Guid userId, string token);
    Task<AuthenticationResultDto> ChangeEmailAsync(Guid userId, string newEmail, string token);
    Task<AuthenticationResultDto> ChangePhoneNumberAsync(string? phoneNumber);
    Task<AuthenticationResultDto> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<AuthenticationResultDto> ConfirmPasswordAsync(string password);
    Task<AuthenticationResultDto> ResetPasswordAsync(UserDto userDto, string token, string newPassword);
    Task<UserPreferencesDto> GetUserPreferencesAsync();
    Task<AuthenticationResultDto> ChangeUserPreferencesAsync(Preferences userPreferences);
    Guid GetCurrentUserId();
    bool IsUserAuthenticated();
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> IsUserEmailConfirmedAsync(UserDto user);
    Task<AuthenticationResultDto> DeleteAccount();
}
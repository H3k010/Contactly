using ContactsManager.Application.DTO;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Enums;
using ContactsManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ContactsManager.Infrastructure.Services.Account;

public class AccountService : IAccountService
{
    private readonly IStringLocalizer<AccountService> _localizer;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICurrentUserService _currentUserService;

    public AccountService(IStringLocalizer<AccountService> localizer,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICurrentUserService currentUserService)
    {
        _localizer = localizer;
        _userManager = userManager;
        _signInManager = signInManager;
        _currentUserService = currentUserService;
    }

    private const int PendingRegistrationLifetimeHours = 24;

    public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        var validationUser = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.Phone
        };

        var userValidationResult = await ValidateUserAsync(validationUser);

        var passwordValidationResult = await ValidatePasswordAsync(validationUser, registerDto.Password);

        var validationErrors = userValidationResult.Errors
            .Concat(passwordValidationResult.Errors)
            .Where(e => e.Code != "DuplicateEmail" && e.Code!="DuplicateUserName").ToList();

        if (validationErrors.Count > 0)
        {
            return new RegisterResult
            {
                Succeeded = false,
                IsEmailDuplicate = false,
                Errors = validationErrors.Select(e => e.Description)
            };
        }

        if (existingUser is not null)
        {
            if (existingUser.EmailConfirmed)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    IsEmailDuplicate = true
                };
            }

            var registrationExpired = !existingUser.RegisteredAt.HasValue ||
                                      existingUser.RegisteredAt.Value
                                          .AddHours(PendingRegistrationLifetimeHours) <= DateTime.UtcNow;

            if (registrationExpired)
            {
                var deleteResult = await _userManager.DeleteAsync(existingUser);

                if (!deleteResult.Succeeded)
                {
                    return new RegisterResult
                    {
                        Succeeded = false,
                        Errors = deleteResult.Errors.Select(e => e.Description)
                    };
                }

                existingUser = null;
            }
        }

        if (existingUser is not null)
        {
            var existingUsernameUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUsernameUser is not null && existingUsernameUser.Id != existingUser.Id)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    Errors = new[] { _userManager.ErrorDescriber.DuplicateUserName(registerDto.UserName).Description }
                };
            }

            existingUser.UserName = registerDto.UserName;
            existingUser.Email = registerDto.Email;
            existingUser.PhoneNumber = registerDto.Phone;
            existingUser.UserPreferences = new Preferences();

            var updateResult = await _userManager.UpdateAsync(existingUser);

            if (!updateResult.Succeeded)
            {
                var otherErrors = updateResult.Errors.Where(e => e.Code != "DuplicateEmail").ToList();

                if (otherErrors.Count > 0)
                {
                    return new RegisterResult
                    {
                        Succeeded = false,
                        Errors = otherErrors.Select(e => e.Description)
                    };
                }

                return new RegisterResult
                {
                    Succeeded = false,
                    IsEmailDuplicate = true
                };
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(existingUser);

            if (!removePasswordResult.Succeeded)
            {
                return new RegisterResult
                {
                    Succeeded = false, Errors = removePasswordResult.Errors.Select(e => e.Description)
                };
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(existingUser, registerDto.Password);

            if (!addPasswordResult.Succeeded)
            {
                return new RegisterResult
                {
                    Succeeded = false, Errors = addPasswordResult.Errors.Select(e => e.Description)
                };
            }

            return new RegisterResult
            {
                Succeeded = true, UserId = existingUser.Id
            };
        }

        // Create a new account if there is no existing one
        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.Phone,
            UserPreferences = new Preferences(),
        };

        var createResult = await _userManager.CreateAsync(user, registerDto.Password);

        if (!createResult.Succeeded)
        {
            var otherErrors = createResult.Errors.Where(e => e.Code != "DuplicateEmail").ToList();

            if (otherErrors.Count > 0)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    IsEmailDuplicate = false,
                    Errors = otherErrors.Select(e => e.Description)
                };
            }

            return new RegisterResult { Succeeded = false, IsEmailDuplicate = true };
        }

        return new RegisterResult
        {
            Succeeded = true,
            UserId = user.Id
        };
    }

    public async Task<AuthenticationResultDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = new List<string> { _localizer["EmailOrPasswordInvalid"] }
            };
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            loginDto.Password,
            isPersistent: loginDto.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = new List<string> { _localizer["EmailOrPasswordInvalid"] }
            };
        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            UserPreferences = user.UserPreferences,
            IsAuthenticated = IsAuthenticated(),
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            UserPreferences = user.UserPreferences,
            IsAuthenticated = IsAuthenticated(),
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public async Task<bool> IsUserEmailConfirmedAsync(UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        return user != null && await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<AuthenticationResultDto> DeleteAccount()
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToDeleteAccount"] }
            };
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToDeleteAccount"] }
            };
        }

        await _signInManager.SignOutAsync();
        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<AuthenticationResultDto> ChangeUserNameAsync(string newUserName)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToChangeUserName"] }
            };
        }

        user.UserName = newUserName;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _signInManager.RefreshSignInAsync(user);

        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<AuthenticationResultDto> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        if (!IsAuthenticated())
        {
            await _signInManager.SignInAsync(user, false);
        }
        else
        {
            await _signInManager.RefreshSignInAsync(user);
        }

        return new AuthenticationResultDto
        {
            Succeeded = result.Succeeded,
            Errors = result.Errors.Select(e => e.Description)
        };
    }

    public async Task<AuthenticationResultDto> ChangeEmailAsync(Guid userId, string newEmail, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToChangeEmail"] }
            };
        }

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _signInManager.RefreshSignInAsync(user);

        return new AuthenticationResultDto
        {
            Succeeded = result.Succeeded,
            Errors = result.Errors.Select(e => e.Description)
        };
    }

    public async Task<AuthenticationResultDto> ChangePhoneNumberAsync(string? phoneNumber)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToChangePhoneNumber"] }
            };
        }

        user.PhoneNumber = phoneNumber;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _signInManager.RefreshSignInAsync(user);

        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<AuthenticationResultDto> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToChangePassword"] }
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _signInManager.RefreshSignInAsync(user);

        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<AuthenticationResultDto> ConfirmPasswordAsync(string password)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToConfirmPassword"] }
            };
        }

        var valid = await _userManager.CheckPasswordAsync(user, password);
        if (!valid)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["InvalidPassword"] }
            };
        }

        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<AuthenticationResultDto> ResetPasswordAsync(UserDto userDto, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToResetPassword"] }
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = result.Errors.Select(e => e.Description)
            };
        }

        return new AuthenticationResultDto { Succeeded = true };
    }

    public async Task<UserPreferencesDto> GetUserPreferencesAsync()
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
        var preferences = user?.UserPreferences;

        var preferencesDto = new UserPreferencesDto
        {
            PageSize = (int)PageSizeOptions.Size25,
            SortBy = nameof(SortByOptions.FullName),
            SortOrder = nameof(SortOrderOptions.ASC)
        };

        if (preferences == null) return preferencesDto;
        preferencesDto.PageSize = (int)preferences.PageSize;
        preferencesDto.SortBy = preferences.SortBy.ToString();
        preferencesDto.SortOrder = preferences.SortOrder.ToString();

        return preferencesDto;
    }

    public Guid GetCurrentUserId()
    {
        return GetUserId();
    }

    public bool IsUserAuthenticated()
    {
        return IsAuthenticated();
    }

    public async Task<AuthenticationResultDto> ChangeUserPreferencesAsync(Preferences userPreferences)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false, Errors = new List<string> { _localizer["FailedToChangeUserPreferences"] }
            };
        }

        user.UserPreferences = userPreferences;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new AuthenticationResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _signInManager.RefreshSignInAsync(user);

        return new AuthenticationResultDto { Succeeded = true };
    }

    private Guid GetUserId()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("Current user is not authenticated.");
        }

        return userId.Value;
    }

    private bool IsAuthenticated()
    {
        return _currentUserService.IsAuthenticated;
    }

    private async Task<IdentityResult> ValidateUserAsync(ApplicationUser user)
    {
        var errors = new List<IdentityError>();

        foreach (var validator in _userManager.UserValidators)
        {
            var result = await validator.ValidateAsync(_userManager, user);

            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors);
            }
        }

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
    }

    private async Task<IdentityResult> ValidatePasswordAsync(ApplicationUser user, string password)
    {
        var errors = new List<IdentityError>();

        foreach (var validator in _userManager.PasswordValidators)
        {
            var result = await validator.ValidateAsync(_userManager, user, password);

            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors);
            }
        }

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
    }
}
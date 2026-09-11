using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace ContactsManager.Infrastructure;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<LocalizedIdentityErrorDescriber> _localizer;

    public LocalizedIdentityErrorDescriber(
        IStringLocalizer<LocalizedIdentityErrorDescriber> localizer)
    {
        _localizer = localizer;
    }

    public override IdentityError DefaultError()
        => CreateError(nameof(DefaultError));

    public override IdentityError ConcurrencyFailure()
        => CreateError(nameof(ConcurrencyFailure));

    public override IdentityError PasswordMismatch()
        => CreateError(nameof(PasswordMismatch));

    public override IdentityError InvalidToken()
        => CreateError(nameof(InvalidToken));

    public override IdentityError LoginAlreadyAssociated()
        => CreateError(nameof(LoginAlreadyAssociated));

    public override IdentityError UserAlreadyHasPassword()
        => CreateError(nameof(UserAlreadyHasPassword));

    public override IdentityError UserLockoutNotEnabled()
        => CreateError(nameof(UserLockoutNotEnabled));

    public override IdentityError RecoveryCodeRedemptionFailed()
        => CreateError(nameof(RecoveryCodeRedemptionFailed));

    public override IdentityError PasswordRequiresDigit()
        => CreateError(nameof(PasswordRequiresDigit));

    public override IdentityError PasswordRequiresLower()
        => CreateError(nameof(PasswordRequiresLower));

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => CreateError(nameof(PasswordRequiresNonAlphanumeric));

    public override IdentityError PasswordRequiresUpper()
        => CreateError(nameof(PasswordRequiresUpper));

    public override IdentityError PasswordTooShort(int length)
        => CreateError(nameof(PasswordTooShort), length);

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        => CreateError(nameof(PasswordRequiresUniqueChars), uniqueChars);

    public override IdentityError DuplicateUserName(string? userName)
        => CreateError(nameof(DuplicateUserName), userName);

    public override IdentityError DuplicateEmail(string email)
        => CreateError(nameof(DuplicateEmail), email);

    public override IdentityError InvalidUserName(string? userName)
        => CreateError(nameof(InvalidUserName), userName);

    public override IdentityError InvalidEmail(string? email)
        => CreateError(nameof(InvalidEmail), email);

    public override IdentityError DuplicateRoleName(string role)
        => CreateError(nameof(DuplicateRoleName), role);

    public override IdentityError InvalidRoleName(string? role)
        => CreateError(nameof(InvalidRoleName), role);

    public override IdentityError UserAlreadyInRole(string role)
        => CreateError(nameof(UserAlreadyInRole), role);

    public override IdentityError UserNotInRole(string role)
        => CreateError(nameof(UserNotInRole), role);

    private IdentityError CreateError(string key, params object?[] arguments)
    {
        return new IdentityError
        {
            Code = key,
            Description = _localizer[key, arguments!].Value
        };
    }
}
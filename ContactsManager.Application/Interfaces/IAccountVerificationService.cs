namespace ContactsManager.Application.Interfaces;

public interface IAccountVerificationService
{
    Task<string> GenerateVerificationLinkAsync(Guid userId, string controller, string action, string scheme);
    Task<string> GenerateConfirmEmailVerificationLinkAsync(string controller, string action, string scheme);

    Task<string> GenerateChangeEmailVerificationLinkAsync(string newEmail, string controller, string action,
        string scheme);

    Task<string> GeneratePasswordResetVeificiationLinkAsync(Guid id, string controller, string action, string scheme);
}
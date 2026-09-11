namespace ContactsManager.Application.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string newEmail, string subject, string message);
}
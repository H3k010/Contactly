using ContactsManager.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ContactsManager.Infrastructure.Services.Email;

public class EmailSenderService : IEmailSenderService
{
    private readonly SmtpOptions _smtpOptions;

    public EmailSenderService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task SendEmailAsync(string newEmail, string subject, string htmlMessage)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
        mimeMessage.To.Add(new MailboxAddress("", newEmail));
        mimeMessage.Subject = subject;
        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpOptions.Server, _smtpOptions.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
            await client.SendAsync(mimeMessage);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
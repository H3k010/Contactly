namespace ContactsManager.Infrastructure.Services.Email;

public class SmtpOptions
{
    public const string SectionName = "SmtpSettings";
    
    public string Server { get; set; } = null!;
    public int Port { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
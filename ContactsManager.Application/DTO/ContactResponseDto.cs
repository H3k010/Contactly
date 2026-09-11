namespace ContactsManager.Application.DTO;

public class ContactResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CompanyName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid ApplicationUserId { get; set; }
}

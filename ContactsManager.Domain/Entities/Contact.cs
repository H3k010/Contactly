using System.Text.Json.Serialization;

namespace ContactsManager.Domain.Entities;

public class Contact
{
    public Guid Id { get; init; }
    public Guid ApplicationUserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    [JsonIgnore]
    public ApplicationUser User { get; set; } = null!;
}
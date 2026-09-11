using System.Text.Json.Serialization;
using ContactsManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public Preferences UserPreferences { get; set; } = new();
    
    public DateTime? RegisteredAt { get; set; }

    [JsonIgnore]
    public ICollection<Contact> Contacts { get; init; } = new List<Contact>();
}


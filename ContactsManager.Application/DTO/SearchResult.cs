using ContactsManager.Domain.Entities;

namespace ContactsManager.Application.DTO;

public class SearchResult
{
    public IEnumerable<Contact> Contacts { get; init; } = new List<Contact>();
    public int TotalCount { get; init; }
    public int FilteredCount { get; init; }
}
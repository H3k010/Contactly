using ContactsManager.Application.DTO;

namespace ContactsManager.Presentation.Models;

public class ContactsViewModel
{
    public IEnumerable<ContactResponseDto> Contacts { get; set; } = [];
    public int TotalContactsCount { get; set; }
    public int FilteredContactsCount { get; set; }
    public int CurrentPageSize { get; set; }
    public int CurrentPageNumber { get; set; }
}
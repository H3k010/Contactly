namespace ContactsManager.Application.DTO;

public class SearchResultDto
{
    public IEnumerable<ContactResponseDto> Contacts { get; init; } = new List<ContactResponseDto>();
    public int TotalCount { get; init; }
    public int FilteredCount { get; init; }
}
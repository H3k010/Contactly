namespace ContactsManager.Application.DTO;

public class UserPreferencesDto
{
    public int PageSize { get; set; }
    public string SortBy { get; set; } = null!;
    public string SortOrder { get; set; } = null!;
}
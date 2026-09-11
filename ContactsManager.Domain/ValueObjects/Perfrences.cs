using ContactsManager.Domain.Enums;

namespace ContactsManager.Domain.ValueObjects;

public class Preferences
{
    public PageSizeOptions PageSize { get; set; } = PageSizeOptions.Size25;
    public SortByOptions SortBy { get; set; } = SortByOptions.FullName;
    public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;
}
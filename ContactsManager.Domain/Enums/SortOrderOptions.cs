// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Domain.Enums;

public enum SortOrderOptions
{
    [Display(Name = "A → Z")]
    ASC,
    [Display(Name = "Z → A")]
    DESC
}
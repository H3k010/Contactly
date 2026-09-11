using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Domain.Enums;

public enum SortByOptions
{
    [Display(Name = "Name")]
    FullName,
    [Display(Name = "Email")]
    Email,
    [Display(Name = "Company")]
    CompanyName,
    [Display(Name = "CreatedAt")]
    CreatedAt
}
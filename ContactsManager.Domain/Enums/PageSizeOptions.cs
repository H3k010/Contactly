using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Domain.Enums;

public enum PageSizeOptions
{
    [Display(Name = "Display25")]
    Size25 = 25,
    [Display(Name = "Display50")]
    Size50 = 50,
    [Display(Name = "Display100")]
    Size100 = 100
}
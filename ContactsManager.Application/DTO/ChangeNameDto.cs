using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ChangeNameDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "UserNameRequired")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "StringLength")]
    [Display(Name = "UserName")]
    public string NewName { get; set; } = null!;
}
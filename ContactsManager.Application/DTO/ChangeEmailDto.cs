using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ChangeEmailDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailFormat")]
    public string Email { get; set; } = null!;
}
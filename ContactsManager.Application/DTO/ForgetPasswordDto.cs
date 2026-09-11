using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ForgetPasswordDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "EmailRequired")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
}
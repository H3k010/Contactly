using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ChangePhoneDto
{
    [Phone(ErrorMessage = "PhoneInvalid")]
    [Display(Name = "PhoneNumber")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class RegisterDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "UserNameRequired")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "StringLength")]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;


    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailFormat")]
    public string Email { get; set; } = null!;


    [Required(ErrorMessage = "PhoneRequired")]
    [Phone(ErrorMessage = "PhoneInvalid")]
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; } = null!;


    [Required(ErrorMessage = "PasswordRequired")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;


    [Required(ErrorMessage = "ConfirmPasswordRequired")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "PasswordMismatch")]
    public string ConfirmPassword { get; set; } = null!;
}
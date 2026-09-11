using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ChangePasswordDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "CurrentPasswordRequired")]
    [DataType(DataType.Password)]
    [Display(Name = "CurrentPassword")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "NewPasswordRequired")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordLength")]
    [DataType(DataType.Password)]
    [Display(Name = "NewPassword")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "ConfirmPasswordRequired")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "ConfirmPasswordMismatch")]
    [Display(Name = "ConfirmPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
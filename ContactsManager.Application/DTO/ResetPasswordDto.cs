using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Required")] public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required"), EmailAddress(ErrorMessage = "EmailFormat")] public string Email { get; set; } = string.Empty;

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
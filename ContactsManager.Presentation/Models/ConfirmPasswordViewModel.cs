using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Presentation.Models;

public class ConfirmPasswordViewModel
{
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordLength")]
    public string Password { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")] public string Action { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string SubmitText { get; set; } = string.Empty;
}
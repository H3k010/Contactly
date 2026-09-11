using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailFormat")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; init; } = null!;


    [Required(ErrorMessage = "PasswordRequired")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = null!;
    
    public bool RememberMe { get; init; }
}
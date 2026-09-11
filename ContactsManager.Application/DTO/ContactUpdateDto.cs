using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTO;

public class ContactUpdateDto
{
    [Required(ErrorMessage = "Required")] public Guid Id { get; init; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "StringLength")]
    [Display(Name = "FirstName")]
    [RegularExpression(@"^\S*$", ErrorMessage = "NoSpacesAllowed")]
    public string FirstName { get; init => field = value.Trim(); } = null!;

    [StringLength(15, ErrorMessage = "StringLength")]
    [Display(Name = "LastName")]
    [RegularExpression(@"^\S*$", ErrorMessage = "NoSpacesAllowed")]
    public string? LastName { get; init => field = value?.Trim(); }
    
    [StringLength(100, ErrorMessage = "StringLength")] 
    public string? Address { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
    [Phone(ErrorMessage = "PhoneInvalid")]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "PhoneNumber")]
    public string PhoneNumber { get; init => field = value.Trim(); } = null!;

    [EmailAddress(ErrorMessage = "EmailFormat")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "EmailAddress")]
    public string? Email { get; init => field = value?.Trim(); }

    [StringLength(50, ErrorMessage = "StringLength")]
    [Display(Name = "Company")]
    public string? CompanyName { get; set; }

    [StringLength(100, ErrorMessage = "StringLength")]
    public string? Notes { get; set; }

    public Guid ApplicationUserId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace ContactsManager.Application.DTO;

public class ContactRequestDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "StringLength")]
    [RegularExpression(@"^\S*$", ErrorMessage = "NoSpacesAllowed")]
    [Display(Name = "FirstName")]
    public string FirstName { get; init => field = value.Trim(); } = null!;

    [StringLength(20, ErrorMessage = "StringLength")]
    [RegularExpression(@"^\S*$", ErrorMessage = "NoSpacesAllowed")]
    [Display(Name = "LastName")]
    public string? LastName { get; init => field = value?.Trim(); }

    [StringLength(100, ErrorMessage = "StringLength")] 
    public string? Address { get; set; }

    [EmailAddress(ErrorMessage = "EmailFormat")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "EmailAddress")]
    public string? Email { get; init => field = value?.Trim(); }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
    [Phone(ErrorMessage = "PhoneInvalid")]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "PhoneNumber")]
    public string PhoneNumber { get; init => field = value.Trim(); } = null!;
    
    [StringLength(50, ErrorMessage = "StringLength")]
    [Display(Name = "Company")]
    public string? CompanyName { get; set; }

    [StringLength(100, ErrorMessage = "StringLength")]
    public string? Notes { get; set; }

    [Ignore]
    public Guid ApplicationUserId { get; set; }
}
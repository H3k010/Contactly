using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.Helper;

public static class ValidationHelper
{
    public static bool Validate(object obj)
    {
        var context = new ValidationContext(obj);
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(obj, context, result, validateAllProperties: true);

        return isValid;
    }
}
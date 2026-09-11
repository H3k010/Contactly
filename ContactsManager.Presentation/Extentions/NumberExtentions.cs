using System.Globalization;

namespace ContactsManager.Presentation.Extentions;

public static class NumberExtentions
{
    public static string ToArabicNumerals(this int number)
    {
        const string arabicDigits = "٠١٢٣٤٥٦٧٨٩";

        return string.Concat(
            number
                .ToString(CultureInfo.InvariantCulture)
                .Select(c => char.IsDigit(c)
                    ? arabicDigits[c - '0'].ToString()
                    : c.ToString())
        );
    }
    
    public static string ToArabicNumerals(this int? number)
    {

        return number.HasValue ? number.Value.ToArabicNumerals() : string.Empty;
    }
    
    
}
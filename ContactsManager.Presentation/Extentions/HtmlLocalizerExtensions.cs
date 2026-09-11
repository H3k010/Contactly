using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Localization;

namespace ContactsManager.Presentation.Extentions;

public static class HtmlLocalizerExtensions
{
    public static string ToHtmlString(this LocalizedHtmlString localizedHtml)
    {
        using var writer = new StringWriter();
        localizedHtml.WriteTo(writer, HtmlEncoder.Default); 
        return writer.ToString();
    }
}
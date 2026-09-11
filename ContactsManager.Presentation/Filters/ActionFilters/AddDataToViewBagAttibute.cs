using ContactsManager.Application.DTO;
using ContactsManager.Application.Interfaces;
using ContactsManager.Presentation.Controllers;
using Ganss.Xss;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Presentation.Filters.ActionFilters;

[AttributeUsage(AttributeTargets.Method)]
public class AddDataToViewBagAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var accountService = serviceProvider.GetRequiredService<IAccountService>();
        return new AddDataToViewBagFilter(accountService);
    }
}

public class AddDataToViewBagFilter : IAsyncActionFilter
{
    private readonly IAccountService _accountService;

    public AddDataToViewBagFilter(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is ContactsController controller)
        {
            controller.ViewBag.SearchFields = new Dictionary<string, string>
            {
                { nameof(ContactResponseDto.FullName), "Name" },
                { nameof(ContactResponseDto.Email), "Email" },
                { nameof(ContactResponseDto.PhoneNumber), "Phone" },
                { nameof(ContactResponseDto.CompanyName), "Company" },
                { nameof(ContactResponseDto.Address), "Address" }
            };
            controller.ViewBag.SearchFieldsAr = new Dictionary<string, string>
            {
                { nameof(ContactResponseDto.FullName), "الاسم" },
                { nameof(ContactResponseDto.Email), "البريد الإلكترونى" },
                { nameof(ContactResponseDto.PhoneNumber), "رقم الهاتف" },
                { nameof(ContactResponseDto.CompanyName), "الشركة" },
                { nameof(ContactResponseDto.Address), "العنوان" }
            };

            const string rawFullName = "<span lang=\"en\">Name</span><span lang=\"ar\">الاسم</span>";
            const string rawEmail = "<span lang=\"en\">Email</span><span lang=\"ar\">البريد الإلكترونى</span>";
            const string rawPhoneNumber = "<span lang=\"en\">Phone</span><span lang=\"ar\">رقم الهاتف</span>";
            const string rawCompanyName = "<span lang=\"en\">Company</span><span lang=\"ar\">الشركة</span>";
            const string rawAddress = "<span lang=\"en\">Address</span><span lang=\"ar\">العنوان</span>";
            const string rawIconContent = "<i class=\"fa-solid fa-clock\"></i>";
            
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedAttributes.Add("class");
            
            var cleanFullName = sanitizer.Sanitize(rawFullName);
            var cleanEmail = sanitizer.Sanitize(rawEmail);
            var cleanPhoneNumber = sanitizer.Sanitize(rawPhoneNumber);
            var cleanCompanyName = sanitizer.Sanitize(rawCompanyName);
            var cleanAddress = sanitizer.Sanitize(rawAddress);
            var cleanIcon = sanitizer.Sanitize(rawIconContent);
            
            var name = new HtmlString(cleanFullName);
            var email = new HtmlString(cleanEmail);
            var phoneNumber = new HtmlString(cleanPhoneNumber);
            var companyName = new HtmlString(cleanCompanyName);
            var address = new HtmlString(cleanAddress);
            var icon = new HtmlString(cleanIcon);
            
            controller.ViewBag.Name = name;
            controller.ViewBag.Email = email;
            controller.ViewBag.Phone = phoneNumber;
            controller.ViewBag.Company = companyName;
            controller.ViewBag.Address = address;
            controller.ViewBag.LastAddedIcon = icon;

            if (context.ActionArguments.TryGetValue("searchBy", out var searchByValue))
            {
                controller.ViewBag.CurrentSearchBy = searchByValue?.ToString();
            }
            else
            {
                controller.ViewBag.CurrentSearchBy = nameof(ContactResponseDto.FullName);
            }

            if (context.ActionArguments.TryGetValue("searchString", out var searchStringValue))
            {
                controller.ViewBag.CurrentSearchString = searchStringValue?.ToString();
            }

            if (context.ActionArguments.TryGetValue("sortBy", out var sortByValue))
            {
                controller.ViewBag.CurrentSortBy = sortByValue?.ToString();
            }
            else
            {
                
                controller.ViewBag.CurrentSortBy = await CheckSortBy(sortByValue?.ToString());
            }

            if (context.ActionArguments.TryGetValue("sortOrder", out var sortOrderValue))
            {
                controller.ViewBag.CurrentSortOrder = sortOrderValue?.ToString();
            }
            else
            {
                controller.ViewBag.CurrentSortOrder = await CheckSortOrder(sortOrderValue?.ToString());
            }
        }
        
        await next();
    }

    private async Task<string> CheckSortBy(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = (await _accountService.GetUserPreferencesAsync()).SortBy;
        }

        return sortBy;
    }

    private async Task<string> CheckSortOrder(string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortOrder))
        {
            sortOrder = (await _accountService.GetUserPreferencesAsync()).SortOrder;
        }

        return sortOrder;
    }
}
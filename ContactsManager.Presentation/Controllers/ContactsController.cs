using System.Text;
using Microsoft.AspNetCore.Mvc;
using ContactsManager.Application.DTO;
using ContactsManager.Application.Extensions;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Enums;
using ContactsManager.Presentation.Filters.ActionFilters;
using ContactsManager.Presentation.Models;
using Microsoft.Extensions.Localization;
using Rotativa.AspNetCore;

namespace ContactsManager.Presentation.Controllers;

[Route("[controller]/[action]")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class ContactsController : Controller
{
    private readonly IStringLocalizer<ContactsController> _localizer;
    private readonly IContactsService _contactsService;
    private readonly IAccountService _accountService;

    public ContactsController(IStringLocalizer<ContactsController> localizer, IContactsService contactsService, IAccountService accountService)
    {
        _localizer = localizer;
        _contactsService = contactsService;
        _accountService = accountService;
    }

    [HttpGet("/[controller]")]
    [AddDataToViewBag]
    public async Task<IActionResult> Index(
        string? searchBy,
        string? searchString,
        string? sortBy,
        SortOrderOptions? sortOrder,
        int pageSize,
        int pageNumber = 1)
    {
        var userPreferences = await _accountService.GetUserPreferencesAsync();

        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = userPreferences.SortBy;
        }

        if (string.IsNullOrEmpty(sortOrder.ToString()))
        {
            sortOrder = Enum.Parse<SortOrderOptions>(userPreferences.SortOrder);
        }

        if (pageSize <= 0)
        {
            pageSize = userPreferences.PageSize;
        }

        var searchResultDto =
            await _contactsService.SearchContactsAsync(searchBy, searchString, sortBy, sortOrder, pageNumber, pageSize);

        var model = new ContactsViewModel
        {
            Contacts = searchResultDto.Contacts,
            TotalContactsCount = searchResultDto.TotalCount,
            FilteredContactsCount = searchResultDto.FilteredCount,
            CurrentPageSize = pageSize,
            CurrentPageNumber = pageNumber,
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ContactRequestDto contactRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        await _contactsService.AddContactAsync(contactRequestDto);
        return RedirectToAction(nameof(Index), "Contacts");
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var contactResponse = await _contactsService.GetContactByIdAsync(id);
        if (contactResponse == null)
        {
            return RedirectToAction(nameof(Index), "Contacts");
        }

        var contactUpdate = contactResponse.ToUpdateDto();

        return View(contactUpdate);
    }

    [HttpPost("{id:Guid}")]
    public async Task<IActionResult> Edit(Guid id, ContactUpdateDto contactUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return View(contactUpdateDto);
        }

        var contactResponseDto = await _contactsService.GetContactByIdAsync(id);
        if (contactResponseDto == null)
        {
            return RedirectToAction(nameof(Index), "Contacts");
        }

        await _contactsService.UpdateContactAsync(contactUpdateDto);
        return RedirectToAction(nameof(Index), "Contacts");
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var contactResponseDto = await _contactsService.GetContactByIdAsync(id);
        if (contactResponseDto == null)
        {
            return RedirectToAction(nameof(Index), "Contacts");
        }

        return View(contactResponseDto);
    }

    [HttpPost("{id:Guid}")]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _contactsService.DeleteContactAsync(id);
        return RedirectToAction(nameof(Index), "Contacts");
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var contactResponseDto = await _contactsService.GetContactByIdAsync(id);
        if (contactResponseDto == null)
        {
            return RedirectToAction(nameof(Index), "Contacts");
        }

        return View(contactResponseDto);
    }

    [HttpGet("{format}")]
    public async Task<IActionResult> Export(string format)
    {
        if (string.Equals(format, "pdf", StringComparison.CurrentCultureIgnoreCase))
        {
            var contacts = await _contactsService.GetAllContactsAsync();
            return new ViewAsPdf("ContactsPdf", contacts, ViewData)
            {
                FileName = "Contacts.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageMargins = new Rotativa.AspNetCore.Options.Margins
                {
                    Top = 10,
                    Bottom = 10,
                    Left = 10,
                    Right = 10
                }
            };
        }

        if (string.Equals(format, "csv", StringComparison.CurrentCultureIgnoreCase))
        {
            var csvStream = await _contactsService.GetCsvContacts();
            return File(csvStream, "application/octet-stream", "Contacts.csv");
        }

        if (string.Equals(format, "json", StringComparison.CurrentCultureIgnoreCase))
        {
            var contacts = await _contactsService.GetJsonContacts();
            return File(Encoding.UTF8.GetBytes(contacts), "application/json", "Contacts.json");
        }

        return BadRequest("Invalid or Unsupported format");
    }

    [HttpGet]
    public IActionResult Import()
    {
        var model = new ImportViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile? importedFile)
    {
        var model = new ImportViewModel();
        if (importedFile == null || importedFile.Length == 0)
        {
            model.UploadError = _localizer["SelectValidFile"];
            return View(model);
        }

        var extension = Path.GetExtension(importedFile.FileName).ToLowerInvariant();
        if (extension != ".csv" && extension != ".json")
        {
            model.UploadError = _localizer["InvalidFileFormat"];
            return View(model);
        }

        await using (var stream = importedFile.OpenReadStream())
        {
            switch (extension)
            {
                case ".csv":
                {
                    var result = await _contactsService.UploadCsvContacts(stream);
                    if (!result.Success)
                    {
                        model.IsSuccess = result.Success;
                        model.UploadError = result.Message;
                        return View(model);
                    }

                    model.IsSuccess = result.Success;
                    model.SuccessUpload = result.Message;
                    model.ContactsAdded = result.ContactsAdded;
                    model.ContactsFailedToAdd = result.ContactsFailedToAdd;
                    break;
                }
                case ".json":
                {
                    var result = await _contactsService.UploadJsonContacts(stream);
                    if (!result.Success)
                    {
                        model.IsSuccess = result.Success;
                        model.UploadError = result.Message;
                        return View(model);
                    }

                    model.IsSuccess = result.Success;
                    model.SuccessUpload = result.Message;
                    model.ContactsAdded = result.ContactsAdded;
                    model.ContactsFailedToAdd = result.ContactsFailedToAdd;
                    break;
                }
                default:
                    model.UploadError = _localizer["InvalidFormat"];
                    return View(model);
            }
        }

        return View(model);
    }
}
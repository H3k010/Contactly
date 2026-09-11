using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ContactsManager.Application.DTO;
using ContactsManager.Application.Extensions;
using ContactsManager.Application.Helper;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Enums;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Localization;

namespace ContactsManager.Infrastructure.Services;

public class ContactsService : IContactsService
{
    private readonly IStringLocalizer<ContactsService> _localizer;
    private readonly IContactsRepository _contactsRepository;
    private readonly ICurrentUserService _currentUserService;

    public ContactsService(IStringLocalizer<ContactsService> localizer, IContactsRepository contactsRepository, ICurrentUserService currentUserService) 
    {
        _localizer = localizer;
        _contactsRepository = contactsRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<ContactResponseDto>> GetAllContactsAsync()
    {
        var userId = GetUserId();
        var contacts = await _contactsRepository.GetAllAsync(userId);
        return contacts.Select(c => c.ToResponseDto()).ToList();
    }

    public async Task<ContactResponseDto> AddContactAsync(ContactRequestDto? contactRequestDto)
    {
        if (contactRequestDto == null)
        {
            throw new ArgumentNullException(nameof(contactRequestDto), @"Contact request cannot be null.");
        }

        var contact = contactRequestDto.ToEntity();
        var userId = GetUserId();
        contact.ApplicationUserId = userId;
        var addedContact = await _contactsRepository.AddAsync(contact);
        return addedContact.ToResponseDto();
    }

    public async Task<ContactResponseDto?> GetContactByIdAsync(Guid? id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id), @"Contact ID cannot be null.");
        }

        var userId = GetUserId();

        var contact = await _contactsRepository.GetByIdAsync(userId, id.Value);
        return contact?.ToResponseDto();
    }

    public async Task<SearchResultDto> SearchContactsAsync(string? searchBy, string? searchString, string? sortBy,
        SortOrderOptions? sortOrder, int pageNumber = 1, int pageSize = 25)
    {
        var userId = GetUserId();
        var searchResult = await _contactsRepository.SearchAsync(userId, searchBy, searchString, sortBy, sortOrder, pageNumber, pageSize);

        return new SearchResultDto
        {
            Contacts = searchResult.Contacts.Select(c => c.ToResponseDto()).ToList(),
            TotalCount = searchResult.TotalCount,
            FilteredCount = searchResult.FilteredCount
        };
    }

    public async Task<MemoryStream> GetCsvContacts()
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

        csvWriter.WriteField(nameof(ContactResponseDto.FirstName));
        csvWriter.WriteField(nameof(ContactResponseDto.LastName));
        csvWriter.WriteField(nameof(ContactResponseDto.Email));
        csvWriter.WriteField(nameof(ContactResponseDto.PhoneNumber));
        csvWriter.WriteField(nameof(ContactResponseDto.CompanyName));
        csvWriter.WriteField(nameof(ContactResponseDto.Address));
        csvWriter.WriteField(nameof(ContactResponseDto.Notes));
        csvWriter.WriteField(nameof(ContactResponseDto.CreatedAt));

        await csvWriter.NextRecordAsync();

        var userId = GetUserId();
        var contacts = await _contactsRepository.GetAllAsync(userId);
        var contactsList = contacts.Select(c => c.ToResponseDto()).OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
            .ToList();
        foreach (var contact in contactsList)
        {
            csvWriter.WriteField(contact.FirstName);
            csvWriter.WriteField(contact.LastName);
            csvWriter.WriteField(contact.Email);
            csvWriter.WriteField(contact.PhoneNumber);
            csvWriter.WriteField(contact.CompanyName);
            csvWriter.WriteField(contact.Address);
            csvWriter.WriteField(contact.Notes);
            csvWriter.WriteField(contact.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<string> GetJsonContacts()
    {
        var userId = GetUserId();
        var contacts = await _contactsRepository.GetAllAsync(userId);
        var contactsList = contacts.Select(c => c.ToResponseDto()).OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
            .ToList();
        var jsonData = contactsList.Select(c => new
        {
            c.FirstName,
            c.LastName,
            c.Email,
            c.PhoneNumber,
            c.CompanyName,
            c.Address,
            c.Notes,
            CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        });

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        return JsonSerializer.Serialize(jsonData, options);
    }

    public async Task<UploadResultDto> UploadCsvContacts(Stream csvStream)
    {
        var configs = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            MissingFieldFound = null,
            Encoding = new UTF8Encoding()
        };
        var addedContacts = 0;
        var failedToAddContacts = 0;
        try
        {
            var streamReader = new StreamReader(csvStream);
            using var csvReader = new CsvReader(streamReader, configs);
            var contactsCsv = csvReader.GetRecords<ContactRequestDto>();
            var contactsList = contactsCsv.ToList();
            if (contactsList.Count == 0)
            {
                return new UploadResultDto
                    { Success = false, Message = _localizer["CSVContactNotFound"] };
            }

            foreach (var contact in contactsList)
            {
                if (!ValidationHelper.Validate(contact))
                {
                    failedToAddContacts++;
                    continue;
                }
                contact.ApplicationUserId = GetUserId();
                await _contactsRepository.AddAsync(contact.ToEntity());
                addedContacts++;
            }
        }
        catch (HeaderValidationException)
        {
            return new UploadResultDto
            {
                Success = false, Message = _localizer["CSVHeaderDoesNotMatch"]
            };
        }
        catch (CsvHelperException)
        {
            return new UploadResultDto { Success = false, Message = _localizer["CSVInvalidFormat"] };
        }
        catch (Exception)
        {
            return new UploadResultDto { Success = false, Message = _localizer["CSVErrors"] };
        }

        return new UploadResultDto
            { 
                Success = true, 
                ContactsAdded = addedContacts,
                ContactsFailedToAdd = failedToAddContacts,
                Message = _localizer["CSVSuccess"]
            };
    }

    public async Task<UploadResultDto> UploadJsonContacts(Stream jsonStream)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        var contactsAdded = 0;
        var failedToAddContacts = 0;

        try
        {
            var contactsList = await JsonSerializer.DeserializeAsync<List<ContactRequestDto>>(jsonStream, options);

            if (contactsList == null || contactsList.Count == 0)
            {
                return new UploadResultDto
                    { Success = false, Message = _localizer["JSONContactNotFound"] };
            }

            var userId = GetUserId();
            foreach (var contact in contactsList)
            {
                if (!ValidationHelper.Validate(contact))
                {
                    failedToAddContacts++;
                    continue;
                }
                contact.ApplicationUserId = userId;
                await _contactsRepository.AddAsync(contact.ToEntity());
                contactsAdded++;
            }
        }
        catch (JsonException)
        {
            return new UploadResultDto { Success = false, Message = _localizer["JSONInvalidFormat"] };
        }
        catch (Exception)
        {
            return new UploadResultDto
                { Success = false, Message = _localizer["JSONErrors"] };
        }

        return new UploadResultDto
            { 
                Success = true, 
                ContactsAdded = contactsAdded,
                ContactsFailedToAdd = failedToAddContacts, 
                Message = _localizer["JSONSuccess"] };
    }

    public async Task<ContactResponseDto> UpdateContactAsync(ContactUpdateDto? contactUpdate)
    {
        ArgumentNullException.ThrowIfNull(contactUpdate);

        var contact = contactUpdate.ToEntity();
        var userId = GetUserId();
        contact.ApplicationUserId = userId;
        var matchingContact = await _contactsRepository.UpdateAsync(contact);

        if (matchingContact == null)
        {
            throw new ArgumentException("Contact to be updated doesn't exist");
        }

        return matchingContact.ToResponseDto();
    }

    public async Task<bool> DeleteContactAsync(Guid? id)
    {
        if (!id.HasValue)
        {
            throw new ArgumentNullException(nameof(id), @"Contact ID cannot be null.");
        }

        var userId = GetUserId();
        var contact = await _contactsRepository.GetByIdAsync(userId, id.Value);
        if (contact == null)
        {
            return false;
        }

        return await _contactsRepository.DeleteAsync(contact);
    }

    private Guid GetUserId()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("Current user is not authenticated.");
        }

        return userId.Value;
    }
}
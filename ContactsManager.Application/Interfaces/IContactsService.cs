using ContactsManager.Application.DTO;
using ContactsManager.Domain.Enums;

namespace ContactsManager.Application.Interfaces;

public interface IContactsService
{
    Task<List<ContactResponseDto>> GetAllContactsAsync();
    Task<ContactResponseDto> AddContactAsync(ContactRequestDto? contactRequestDto);
    Task<ContactResponseDto?> GetContactByIdAsync(Guid? id);
    Task<SearchResultDto> SearchContactsAsync(string? searchBy, string? searchString,
        string? sortBy, SortOrderOptions? sortOrder, int pageNumber, int pageSize);
    Task<MemoryStream> GetCsvContacts();
    Task<string> GetJsonContacts();
    Task<UploadResultDto> UploadCsvContacts(Stream csvStream);
    Task<UploadResultDto> UploadJsonContacts(Stream jsonStream);
    Task<ContactResponseDto> UpdateContactAsync(ContactUpdateDto contactUpdateDto);
    Task<bool> DeleteContactAsync(Guid? id);
}
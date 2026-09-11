using ContactsManager.Application.DTO;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Enums;

namespace ContactsManager.Application.Interfaces;

public interface IContactsRepository
{
    Task<List<Contact>> GetAllAsync(Guid userId);
    Task<Contact> AddAsync(Contact contact);
    Task<Contact?> GetByIdAsync(Guid userId, Guid id);
    Task<SearchResult> SearchAsync(Guid userId, string? searchBy,
        string? searchString, string? sortBy, SortOrderOptions? sortOrder,
        int pageNumber, int pageSize);
    Task<Contact?> UpdateAsync(Contact contact);
    Task<bool> DeleteAsync(Contact contact);
}
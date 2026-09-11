using ContactsManager.Application.DTO;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Enums;
using ContactsManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;

public class ContactsRepository : IContactsRepository
{
    private readonly AppDbContext _context;

    public ContactsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Contact>> GetAllAsync(Guid userId)
    {
        return await _context.Contacts.AsNoTracking()
            .Where(c => c.ApplicationUserId == userId).ToListAsync();
    }

    public async Task<Contact> AddAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact?> GetByIdAsync(Guid userId, Guid id)
    {
        return await _context.Contacts.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUserId == userId);
    }
    

    public async Task<SearchResult> SearchAsync(Guid userId,
        string? searchBy,
        string? searchString,
        string? sortBy,
        SortOrderOptions? sortOrder,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Contacts.AsNoTracking()
            .Where(c => c.ApplicationUserId == userId);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim();
            query = searchBy switch
            {
                nameof(ContactResponseDto.FullName) => query.Where(contact =>
                    (contact.FirstName + " " + contact.LastName).Contains(searchString)),
                nameof(ContactResponseDto.CompanyName) => query.Where(contact =>
                    contact.CompanyName.Contains(searchString)),
                nameof(ContactResponseDto.Address) => query.Where(contact =>
                    contact.Address.Contains(searchString)),
                nameof(ContactResponseDto.Email) => query.Where(contact =>
                    contact.Email.Contains(searchString)),
                nameof(ContactResponseDto.PhoneNumber) => query.Where(contact =>
                    contact.PhoneNumber.Contains(searchString)),

                _ => query.Where(contact =>
                    (contact.FirstName + " " + contact.LastName).Contains(searchString))
            };
        }
        
        query = (sortBy, sortOrder) switch
        {
            // ASC
            (nameof(ContactResponseDto.FullName), SortOrderOptions.ASC) => query
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ThenBy(c => c.Id),
            (nameof(ContactResponseDto.CompanyName), SortOrderOptions.ASC) => query.OrderBy(c => c.CompanyName)
                .ThenBy(c => c.Id),
            (nameof(ContactResponseDto.Address), SortOrderOptions.ASC) => query.OrderBy(u => u.Address)
                .ThenBy(u => u.Id),
            (nameof(ContactResponseDto.Email), SortOrderOptions.ASC) =>
                query.OrderBy(u => u.Email).ThenBy(u => u.Id),
            (nameof(ContactResponseDto.CreatedAt), SortOrderOptions.ASC) => query.OrderBy(u => u.CreatedAt)
                .ThenBy(u => u.Id),

            // DESC
            (nameof(ContactResponseDto.FullName), SortOrderOptions.DESC) => query
                .OrderByDescending(c => c.FirstName)
                .ThenByDescending(c => c.LastName)
                .ThenBy(c => c.Id),
            (nameof(ContactResponseDto.CompanyName), SortOrderOptions.DESC) => query
                .OrderByDescending(c => c.CompanyName).ThenBy(c => c.Id),
            (nameof(ContactResponseDto.Address), SortOrderOptions.DESC) => query.OrderByDescending(c => c.Address)
                .ThenBy(c => c.Id),
            (nameof(ContactResponseDto.Email), SortOrderOptions.DESC) => query.OrderByDescending(c => c.Email)
                .ThenBy(c => c.Id),
            (nameof(ContactResponseDto.CreatedAt), SortOrderOptions.DESC) => query.OrderByDescending(c => c.CreatedAt)
                .ThenBy(c => c.Id),

            _ => query.OrderByDescending(c => c.CreatedAt)
        };
        var contacts = await query.ToListAsync();
        var filteredCount = contacts.Count;
        var totalCount = await _context.Contacts.AsNoTracking().Where(c => c.ApplicationUserId == userId).CountAsync();

        return new SearchResult
        {
            Contacts = contacts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            TotalCount = totalCount,
            FilteredCount = filteredCount
        };
    }

    public async Task<Contact?> UpdateAsync(Contact contact)
    {
        var existingContact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == contact.Id);
        if (existingContact == null)
        {
            return null;
        }

        existingContact.FirstName = contact.FirstName;
        existingContact.LastName = contact.LastName;
        existingContact.Address = contact.Address;
        existingContact.Email = contact.Email;
        existingContact.PhoneNumber = contact.PhoneNumber;
        existingContact.CompanyName = contact.CompanyName;
        existingContact.Notes = contact.Notes;
        existingContact.ApplicationUserId = contact.ApplicationUserId;

        await _context.SaveChangesAsync();
        return existingContact;
    }

    public async Task<bool> DeleteAsync(Contact contact)
    {
        _context.Contacts.Remove(contact);

        return await _context.SaveChangesAsync() > 0;
    }
}
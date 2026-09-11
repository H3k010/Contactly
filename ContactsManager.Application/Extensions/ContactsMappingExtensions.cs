using ContactsManager.Application.DTO;
using ContactsManager.Domain.Entities;

namespace ContactsManager.Application.Extensions;

public static class ContactsMappingExtensions
{
    public static Contact ToEntity(this ContactRequestDto contactRequestDto)
    {
        return new Contact
        {
            
            Id = Guid.NewGuid(),
            FirstName = contactRequestDto.FirstName,
            LastName = contactRequestDto.LastName ?? string.Empty,
            Email = contactRequestDto.Email ?? string.Empty,
            Address = contactRequestDto.Address ?? string.Empty,
            PhoneNumber = contactRequestDto.PhoneNumber,
            CompanyName = contactRequestDto.CompanyName ?? string.Empty,
            Notes = contactRequestDto.Notes ?? string.Empty,
            ApplicationUserId = contactRequestDto.ApplicationUserId
            
        };
    }

    public static Contact ToEntity(this ContactResponseDto contactResponseDto)
    {
        return new Contact
        {
            Id = contactResponseDto.Id,
            FirstName = contactResponseDto.FirstName,
            LastName = contactResponseDto.LastName,
            Address = contactResponseDto.Address,
            Email = contactResponseDto.Email,
            Notes = contactResponseDto.Notes,
            PhoneNumber = contactResponseDto.PhoneNumber,
            CompanyName = contactResponseDto.CompanyName,
            ApplicationUserId = contactResponseDto.ApplicationUserId
        };
    }

    public static Contact ToEntity(this ContactUpdateDto contactUpdateDto)
    {
        return new Contact
        {
            Id = contactUpdateDto.Id,
            FirstName = contactUpdateDto.FirstName,
            LastName = contactUpdateDto.LastName ?? string.Empty,
            Address = contactUpdateDto.Address ?? string.Empty,
            Email = contactUpdateDto.Email ?? string.Empty,
            Notes = contactUpdateDto.Notes ?? string.Empty,
            CompanyName = contactUpdateDto.CompanyName ?? string.Empty,
            PhoneNumber = contactUpdateDto.PhoneNumber,
            ApplicationUserId = contactUpdateDto.ApplicationUserId
        };
    }

    public static ContactResponseDto ToResponseDto(this Contact contact)
    {
        return new ContactResponseDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Address = contact.Address,
            Email = contact.Email,
            Notes = contact.Notes,
            PhoneNumber = contact.PhoneNumber,
            CompanyName = contact.CompanyName,
            ApplicationUserId = contact.ApplicationUserId,
            CreatedAt = contact.CreatedAt,
            UpdatedAt = contact.UpdatedAt
        }; 
    }
    public static ContactUpdateDto ToUpdateDto(this ContactResponseDto contactResponseDto)
    {
        return new ContactUpdateDto
        {
            Id = contactResponseDto.Id,
            FirstName = contactResponseDto.FirstName,
            LastName = contactResponseDto.LastName,
            Address = contactResponseDto.Address,
            Email = contactResponseDto.Email,
            Notes = contactResponseDto.Notes,
            PhoneNumber = contactResponseDto.PhoneNumber,
            CompanyName = contactResponseDto.CompanyName,
            ApplicationUserId = contactResponseDto.ApplicationUserId
        };
    }
}
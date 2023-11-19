using Contacts.WebAPI.Domain;

namespace Contacts.WebAPI.Infrastructure;

public interface IContactsRepository
{
    Task<(IEnumerable<Contact>, PaginationMetadata)> GetContactsAsync(string? search, string? lastName, string? orderBy, bool? desc, int pageNumber, int pageSize);
    Task<Contact?> GetContactAsync(int id);
    Task CreateContactAsync(Contact contact);
    Task<bool> UpdateContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(int id);
}
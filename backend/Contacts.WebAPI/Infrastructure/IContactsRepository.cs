using Contacts.WebAPI.Domain;

namespace Contacts.WebAPI.Infrastructure;

public interface IContactsRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync(string? search, string? lastName, string? orderBy, bool? desc);
    Task<Contact?> GetContactAsync(int id);
    Task CreateContactAsync(Contact contact);
    Task<bool> UpdateContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(int id);
}
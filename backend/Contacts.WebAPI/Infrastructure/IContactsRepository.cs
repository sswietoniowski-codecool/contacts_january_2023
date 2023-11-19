using Contacts.WebAPI.Domain;

namespace Contacts.WebAPI.Infrastructure;

public interface IContactsRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync(string? search);
    Contact? GetContact(int id);
    void CreateContact(Contact contact);
    bool UpdateContact(Contact contact);
    bool DeleteContact(int id);
}
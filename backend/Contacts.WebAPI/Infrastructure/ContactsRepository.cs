using Contacts.WebAPI.Domain;
using Contacts.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Contacts.WebAPI.Infrastructure;

public class ContactsRepository : IContactsRepository
{
    private readonly ContactsDbContext _dbContext;

    public ContactsRepository(ContactsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<(IEnumerable<Contact>, PaginationMetadata)> GetContactsAsync(string? search, string? lastName,
        string? orderBy, bool? desc,
        int pageNumber, int pageSize)
    {
        var query = _dbContext.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.LastName.Contains(search) || c.FirstName.Contains(search) || c.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(c => c.LastName == lastName);
        }

        var totalItemCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            if (orderBy.Equals(nameof(ContactDto.LastName), StringComparison.OrdinalIgnoreCase))
            {
                query = desc == true
                    ? query.OrderByDescending(c => c.LastName)
                    : query.OrderBy(c => c.LastName);
            }
            else
            {
                throw new ArgumentException("Invalid orderBy value", nameof(orderBy));
            }
        }

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageNumber, pageSize);

        var collectionToReturn = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<Contact?> GetContactAsync(int id)
    {
        return await _dbContext.Contacts
            .Include(c => c.Phones)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateContactAsync(Contact contact)
    {
        _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateContactAsync(Contact contact)
    {
        var contactFromDb = await _dbContext.Contacts.SingleOrDefaultAsync(c => c.Id == contact.Id);

        if (contactFromDb is null)
        {
            return false;
        }

        contactFromDb.FirstName = contact.FirstName;
        contactFromDb.LastName = contact.LastName;
        contactFromDb.Email = contact.Email;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteContactAsync(int id)
    {
        var contact = await _dbContext.Contacts.SingleOrDefaultAsync(c => c.Id == id);

        if (contact is null)
        {
            return false;
        }

        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
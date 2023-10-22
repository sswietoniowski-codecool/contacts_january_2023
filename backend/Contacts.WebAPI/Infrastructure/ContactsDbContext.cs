using Contacts.WebAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace Contacts.WebAPI.Infrastructure;

public class ContactsDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Phone> Phones => Set<Phone>();

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>().HasData
        (
            new Contact
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jkowalski@u.pl"
            },
            new Contact
            {
                Id = 2,
                FirstName = "Adam",
                LastName = "Nowak [BD]",
                Email = "anowak@u.pl"

            }
        );

        modelBuilder.Entity<Phone>().HasData
        (
            new Phone { Id = 1, Number = "111-111-1111", Description = "Domowy", ContactId = 1 },
            new Phone { Id = 2, Number = "222-222-2222", Description = "Służbowy", ContactId = 1 }
        );
    }
}
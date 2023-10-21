using Contacts.WebAPI.Domain;

namespace Contacts.WebAPI.Infrastructure
{
    public class DataService
    {
        public List<Contact> Contacts { get; }

        public DataService()
        {
            Contacts = new()
            {
                new Contact
                {
                    Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jkowalski@u.pl",
                    Phones = new List<Phone>()
                    {
                        new Phone() { Id = 1, Number = "111-111-1111", Description = "Domowy" },
                        new Phone() { Id = 2, Number = "222-222-2222", Description = "Służbowy" }
                    }
                },
                new Contact {Id = 2, FirstName = "Adam", LastName = "Nowak", Email = "anowak@u.pl"}
            };
        }
    }
}

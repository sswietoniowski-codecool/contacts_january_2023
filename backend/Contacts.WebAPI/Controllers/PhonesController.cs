using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contacts.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/contacts/{contactId:int}/phones")]
    public class PhonesController : ControllerBase
    {
        private readonly ContactsDbContext _dbContext;

        public PhonesController(ContactsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET api/contacts/1/phones
        [HttpGet]
        public ActionResult<IEnumerable<PhoneDto>> GetPhones(int contactId)
        {
            var contact = _dbContext.Contacts
                .Include(c => c.Phones)
                .SingleOrDefault(c => c.Id == contactId);

            if (contact is null)
            {
                return NotFound();
            }

            var phonesDto = contact.Phones.Select(p =>
                new PhoneDto()
                {
                    Id = p.Id,
                    Number = p.Number,
                    Description = p.Description
                });

            return Ok(phonesDto);
        }

        // GET api/contacts/1/phones/1
        [HttpGet("{phoneId:int}")]
        public ActionResult<PhoneDto> GetPhone(int contactId, int phoneId)
        {
            var phone = _dbContext.Phones
                .SingleOrDefault(p => p.Id == phoneId && p.ContactId == contactId);

            if (phone is null)
            {
                return NotFound();
            }

            var phoneDto = new PhoneDto()
            {
                Id = phone.Id,
                Number = phone.Number,
                Description = phone.Description
            };

            return Ok(phoneDto);
        }
    }
}

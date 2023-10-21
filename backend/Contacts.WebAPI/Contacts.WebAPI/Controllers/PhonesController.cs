using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/contacts/{contactId:int}/phones")]
    public class PhonesController : ControllerBase
    {
        private readonly DataService _dataService;

        public PhonesController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PhoneDto>> GetPhones(int contactId)
        {
            var contact = _dataService.Contacts.SingleOrDefault(c => c.Id == contactId);

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
    }
}

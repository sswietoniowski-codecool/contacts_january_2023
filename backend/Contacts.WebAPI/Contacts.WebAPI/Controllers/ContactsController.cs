using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly DataService _dataService;

    public ContactsController(DataService dataService)
    {
        _dataService = dataService;
    }

    // GET api/contacts
    [HttpGet]
    public ActionResult<IEnumerable<ContactDto>> GetAll()
    {
        var contactsDto = _dataService.Contacts.Select(c => new ContactDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email
        });

        return Ok(contactsDto);
    }

    // GET api/contacts/1
    [HttpGet("{id:int}")]
    public ActionResult<ContactDto> GetById(int id)
    {
        var contact = _dataService.Contacts.SingleOrDefault(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactDto = new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email
        };

        return Ok(contactDto);
    }
}
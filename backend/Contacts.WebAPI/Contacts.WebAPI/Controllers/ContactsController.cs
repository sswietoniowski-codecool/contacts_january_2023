using Contacts.WebAPI.Domain;
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
    // GET api/contacts?search=ski
    [HttpGet]
    public ActionResult<IEnumerable<ContactDto>> GetContacts([FromQuery] string? search)
    {
        var query = _dataService.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.LastName.Contains(search));
        }

        var contactsDto = query.Select(c => new ContactDto
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
    public ActionResult<ContactDto> GetContact(int id)
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

    // POST api/contacts
    [HttpPost]
    public IActionResult CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
    {
        if (contactForCreationDto.FirstName == contactForCreationDto.LastName)
        {
            ModelState.AddModelError("wrongName", "First name and last name cannot be the same.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var maxId = _dataService.Contacts.Max(c => c.Id);

        var contact = new Contact
        {
            Id = maxId + 1,
            FirstName = contactForCreationDto.FirstName,
            LastName = contactForCreationDto.LastName,
            Email = contactForCreationDto.Email
        };

        _dataService.Contacts.Add(contact);

        var contactDto = new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email
        };

        return CreatedAtAction(nameof(GetContact), new {id = contact.Id}, contactDto);
    }
}
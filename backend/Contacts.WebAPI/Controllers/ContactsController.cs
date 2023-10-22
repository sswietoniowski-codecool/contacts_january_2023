using Contacts.WebAPI.Domain;
using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contacts.WebAPI.Controllers;

[ApiController]
[Route("api/contacts")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ContactsController : ControllerBase
{
    private readonly ContactsDbContext _dbContext;

    public ContactsController(ContactsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // GET api/contacts
    // GET api/contacts?search=ski
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ContactDto>> GetContacts([FromQuery] string? search)
    {
        var query = _dbContext.Contacts.AsQueryable();

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ContactDetailsDto> GetContact(int id)
    {
        var contact = _dbContext.Contacts
            .Include(c => c.Phones)
            .SingleOrDefault(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactDetailsDto = new ContactDetailsDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            Phones = contact.Phones.Select(p => new PhoneDto
            {
                Id = p.Id,
                Number = p.Number,
                Description = p.Description
            })
        };

        return Ok(contactDetailsDto);
    }

    // POST api/contacts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        var contact = new Contact
        {
            FirstName = contactForCreationDto.FirstName,
            LastName = contactForCreationDto.LastName,
            Email = contactForCreationDto.Email
        };

        _dbContext.Contacts.Add(contact);
        _dbContext.SaveChanges();

        var contactDto = new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email
        };

        return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
    }

    // PUT api/contacts/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
    {
        var contact = _dbContext.Contacts.SingleOrDefault(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        contact.FirstName = contactForUpdateDto.FirstName;
        contact.LastName = contactForUpdateDto.LastName;
        contact.Email = contactForUpdateDto.Email;

        _dbContext.SaveChanges();

        return NoContent();
    }

    // DELETE api/contacts/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteContact(int id)
    {
        var contact = _dbContext.Contacts.SingleOrDefault(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        _dbContext.Contacts.Remove(contact);
        _dbContext.SaveChanges();

        return NoContent();
    }

    // PATCH api/contacts/{id}
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult PartiallyUpdateContact(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
    {
        var contact = _dbContext.Contacts.SingleOrDefault(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactToBePatched = new ContactForUpdateDto()
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
        };

        patchDocument.ApplyTo(contactToBePatched, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(contactToBePatched))
        {
            return BadRequest(ModelState);
        }

        contact.FirstName = contactToBePatched.FirstName;
        contact.LastName = contactToBePatched.LastName;
        contact.Email = contactToBePatched.Email;

        _dbContext.SaveChanges();

        return NoContent();
    }
}
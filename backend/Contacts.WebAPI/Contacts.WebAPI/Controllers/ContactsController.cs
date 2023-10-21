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

    [HttpGet()]
    public ActionResult<IEnumerable<ContactDto>> Get()
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
}
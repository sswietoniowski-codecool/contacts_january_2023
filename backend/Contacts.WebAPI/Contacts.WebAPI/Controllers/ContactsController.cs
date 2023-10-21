using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers;

[ApiController]
public class ContactsController : ControllerBase
{
    private readonly DataService _dataService;

    public ContactsController(DataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("api/contacts")]
    public IActionResult Get()
    {
        return new JsonResult(
            _dataService.Contacts
        );
    }
}
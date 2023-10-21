using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers;

[ApiController]
public class ContactsController : Controller
{
    [HttpGet("api/contacts")]
    public IActionResult Get()
    {
        return new JsonResult(
            DataService.Instance.Contacts
        );
    }
}
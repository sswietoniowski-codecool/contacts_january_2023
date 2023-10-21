using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers;

[ApiController]
public class ContactsController : Controller
{
    [HttpGet("api/contacts")]
    public IActionResult Get()
    {
        return new JsonResult(
            new List<object>()
            {
                new { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jkowalski@u.pl"},
                new { Id = 2, FirstName = "Adam", LastName = "Nowak", Email = "anowak@u.pl"}
            }
        );
    }
}
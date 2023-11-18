using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebAPI.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetImages()
    {
        throw new NotImplementedException("Not implemented yet");
    }
}
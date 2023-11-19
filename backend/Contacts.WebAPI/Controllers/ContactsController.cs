using AutoMapper;
using Contacts.WebAPI.Configurations.Options;
using Contacts.WebAPI.Domain;
using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Contacts.WebAPI.Controllers;

[ApiController]
[Route("api/contacts")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ContactsController : ControllerBase
{
    private readonly IContactsRepository _repository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ContactsController> _logger;
    private readonly CorsConfiguration _corsConfiguration;

    public ContactsController(IContactsRepository repository, IMapper mapper, IMemoryCache memoryCache,
        ILogger<ContactsController> logger, IOptions<CorsConfiguration> corsOptions)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _corsConfiguration = corsOptions.Value ?? throw new ArgumentException(nameof(corsOptions));
    }

    // GET api/contacts
    // GET api/contacts?q=ski
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts([FromQuery(Name="q")] string? search,
        [FromQuery] string? lastName,
        [FromQuery] string? orderBy,
        [FromQuery] bool? desc)
    {
        var origins = _corsConfiguration.Origins;

        var origin = Request.Headers["Origin"].ToString();

        if (origins.Contains(origin))
        {
            _logger.LogInformation("Request from {source} is allowed", origin);
        }
        else
        {
            _logger.LogWarning("Request from {source} is not allowed", origin);
        }

        try
        {
            var contacts = await _repository.GetContactsAsync(search, lastName, orderBy, desc);

            var contactsDto = _mapper.Map<IEnumerable<ContactDto>>(contacts);

            return Ok(contactsDto);
        }
        catch (Exception exception)
        {
            // TODO: log the exception

            return Problem("Please try again later...", statusCode: 500);
        }
    }

    // GET api/contacts/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    [ResponseCache(CacheProfileName = "Any-60")]
    public async Task<ActionResult<ContactDetailsDto>> GetContact(int id)
    {
        _logger.LogInformation("Getting contact with id {id}", id);

        var cacheKey = $"{nameof(ContactsController)}-{nameof(GetContact)}-{id}";

        if (!_memoryCache.TryGetValue<ContactDetailsDto>(cacheKey, out var contactDto))
        {
            _logger.LogWarning("Contact with id {id} was not found in cache. Retrieving from database", id);

            var contact = await _repository.GetContactAsync(id);

            if (contact is not null)
            {
                contactDto = _mapper.Map<ContactDetailsDto>(contact);

                _memoryCache.Set(cacheKey, contactDto, TimeSpan.FromSeconds(60));
            }
        }

        if (contactDto is null)
        {
            _logger.LogError("Contact with id {id} was not found in database", id);

            return NotFound();
        }

        return Ok(contactDto);
    }

    // POST api/contacts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
    {
        if (contactForCreationDto.FirstName == contactForCreationDto.LastName)
        {
            ModelState.AddModelError("wrongName", "First name and last name cannot be the same.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contact = _mapper.Map<Contact>(contactForCreationDto);

        await _repository.CreateContactAsync(contact);

        var contactDto = _mapper.Map<ContactDto>(contact);

        return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
    }

    // PUT api/contacts/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
    {
        var contact = _mapper.Map<Contact>(contactForUpdateDto);
        contact.Id = id;

        var success = await _repository.UpdateContactAsync(contact);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE api/contacts/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var success = await _repository.DeleteContactAsync(id);

        return success ? NoContent() : NotFound();
    }

    // PATCH api/contacts/{id}
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PartiallyUpdateContact(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
    {
        var contact = await _repository.GetContactAsync(id);

        if (contact is null)
        {
            return NotFound();
        }

        var contactToBePatched = _mapper.Map<ContactForUpdateDto>(contact);

        patchDocument.ApplyTo(contactToBePatched, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(contactToBePatched))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(contactToBePatched, contact);

        var success = await _repository.UpdateContactAsync(contact);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    //[HttpGet("loop")]
    //public ActionResult<Contact> GetLoop()
    //{
    //    // demo method to demonstrate the loop problem: 
    //    // https://dotnetcoretutorials.com/fixing-json-self-referencing-loop-exceptions/
    //    var contact = _dbContext.Contacts
    //        .Include(c => c.Phones)
    //        .SingleOrDefault(c => c.Id == 1);

    //    return Ok(contact);
    //}
}
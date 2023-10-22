namespace Contacts.WebAPI.DTOs;

public class ContactDetailsDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public IEnumerable<PhoneDto> Phones { get; set; } = Enumerable.Empty<PhoneDto>();
}
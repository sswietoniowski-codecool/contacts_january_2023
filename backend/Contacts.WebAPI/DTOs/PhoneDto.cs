namespace Contacts.WebAPI.DTOs;

public class PhoneDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
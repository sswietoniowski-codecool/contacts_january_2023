using System.ComponentModel.DataAnnotations;

namespace Contacts.WebAPI.DTOs;

public class ContactForCreationDto
{
    [Required]
    [MaxLength(32)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(64)]
    public string LastName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
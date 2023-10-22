using System.ComponentModel.DataAnnotations;

namespace Contacts.WebAPI.Domain;

public class Contact
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(32)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
using System.ComponentModel.DataAnnotations;

namespace Contacts.WebAPI.Configurations.Options;

public class CorsConfiguration
{
    [Required]
    [MinLength(1)]
    public string[] Origins { get; set; } = Array.Empty<string>();
}
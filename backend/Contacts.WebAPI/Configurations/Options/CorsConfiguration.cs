namespace Contacts.WebAPI.Configurations.Options;

public class CorsConfiguration
{
    public string[] Origins { get; set; } = Array.Empty<string>();
}
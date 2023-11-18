using Client.Console.DTOs;
using System.Text.Json;

var httpClient = new HttpClient();

Console.WriteLine("Get Contacts: \n");

var response = await httpClient.GetAsync("http://localhost:5000/api/contacts");

response.EnsureSuccessStatusCode();

var content = await response.Content.ReadAsStringAsync();

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
var contacts = JsonSerializer.Deserialize<IEnumerable<ContactDto>>(content, jsonSerializerOptions);

contacts ??= new List<ContactDto>();

foreach (var contact in contacts)
{
    Console.WriteLine($"{contact.Id} {contact.FirstName} {contact.LastName} {contact.Email}");
}

Console.WriteLine("\nGet Contact:\n");

var id = 1;
response = await httpClient.GetAsync($"http://localhost:5000/api/contacts/{id}");

response.EnsureSuccessStatusCode();

content = await response.Content.ReadAsStringAsync();

var contactDto = JsonSerializer.Deserialize<ContactDetailsDto>(content)!;

Console.WriteLine($"{contactDto.Id} {contactDto.FirstName} {contactDto.LastName} {contactDto.Email}");


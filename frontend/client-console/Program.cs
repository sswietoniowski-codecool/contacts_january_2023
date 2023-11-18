using Client.Console.DTOs;
using System.Text;
using System.Text.Json;

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5000");

Console.WriteLine("Create Contact:\n");

var contactForCreationDto = new ContactForCreationDto
{
    FirstName = "John",
    LastName = "Doe",
    Email = "jdoe@unknown.com"
};

var contactForCreationDtoJson = JsonSerializer.Serialize(contactForCreationDto);

var request = new HttpRequestMessage(HttpMethod.Post, "api/contacts");
request.Content = new StringContent(contactForCreationDtoJson, Encoding.UTF8, "application/json");

var response = await httpClient.SendAsync(request);

response.EnsureSuccessStatusCode();

var cratedContactJson = await response.Content.ReadAsStringAsync();

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
var createdContact = JsonSerializer.Deserialize<ContactDto>(cratedContactJson, jsonSerializerOptions);

Console.WriteLine($"{createdContact.Id} {createdContact.FirstName} {createdContact.LastName} {createdContact.Email}");

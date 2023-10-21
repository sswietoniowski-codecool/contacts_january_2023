namespace Contacts.WebAPI.Domain
{
    public class Phone
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

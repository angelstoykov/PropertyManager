namespace PropertyManager.Application.Configuration
{
    public enum ClientSearchField
    {
        Name,
        Email,
        Phone
    }

    public class ClientSearchOptions
    {
        public const string SectionName = "ClientSearch";

        public string[] Fields { get; set; } = ["Name", "Email", "Phone"];
    }
}

namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientRentedPropertyDto
    {
        public int PropertyId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}

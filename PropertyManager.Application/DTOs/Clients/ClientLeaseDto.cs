namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientLeaseDto
    {
        public int LeaseId { get; set; }
        public DateTime StartDate { get; set; }
        public IReadOnlyList<ClientRentedUnitDto> Units { get; set; } = [];
    }
}

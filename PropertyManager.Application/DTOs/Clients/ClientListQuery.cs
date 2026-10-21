using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientListQuery
    {
        public string? Search { get; set; }
        public string? Tab { get; set; }
        public bool SearchAll { get; set; }
        public string? Status { get; set; }
        public ClientType? ClientType { get; set; }
        public int? BuildingId { get; set; }
    }
}

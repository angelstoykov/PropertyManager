using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientListItemDto
    {
        public int Id { get; set; }
        public ClientType ClientType { get; set; }
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}

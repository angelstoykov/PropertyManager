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
        public bool IsActive { get; set; }
        public bool HasLinkedRecords { get; set; }
        public List<ClientRoleDto> Roles { get; set; } = [];
        public List<string> RentedUnitLabels { get; set; } = [];
    }
}

using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientRoleDto
    {
        public ClientRoleType Role { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}

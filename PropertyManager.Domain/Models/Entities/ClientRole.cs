using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Models.Entities
{
    public class ClientRole
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public ClientRoleType Role { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

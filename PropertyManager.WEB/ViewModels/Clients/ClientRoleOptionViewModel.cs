using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientRoleOptionViewModel
    {
        public ClientRoleType Role { get; set; }
        public DateTime? StartDate { get; set; }
        public bool HasRole { get; set; }
        public bool IsActive { get; set; }
    }
}

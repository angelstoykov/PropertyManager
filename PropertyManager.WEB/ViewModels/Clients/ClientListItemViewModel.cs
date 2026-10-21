using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientListItemViewModel
    {
        public int Id { get; set; }
        public ClientType ClientType { get; set; }
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool HasLinkedRecords { get; set; }
        public string RentedUnits { get; set; } = string.Empty;
        public IReadOnlyList<ClientRoleBadgeViewModel> Roles { get; set; } = [];
    }
}

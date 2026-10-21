using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public static class ClientRolePresentation
    {
        public static ClientRoleBadgeViewModel ToBadge(ClientRoleDto role) => new()
        {
            Label = role.Role.ToString(),
            IsActive = role.IsActive,
            CssClass = role.Role switch
            {
                ClientRoleType.Owner => "bg-primary",
                ClientRoleType.Tenant => "bg-success",
                ClientRoleType.Contractor => "bg-warning text-dark",
                _ => "bg-secondary"
            }
        };
    }
}

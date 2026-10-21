using System.Security.Claims;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Constants;

public static class ClientAccess
{
    public const string ClaimType = "client_access";

    public static readonly IReadOnlyList<ClientRoleType> All =
    [
        ClientRoleType.Owner,
        ClientRoleType.Tenant,
        ClientRoleType.Contractor
    ];

    public static IReadOnlyList<ClientRoleType> Resolve(ClaimsPrincipal user) =>
        Resolve(
            user.IsInRole(Roles.Admin),
            user.IsInRole(Roles.User),
            user.FindAll(ClaimType).Select(claim => claim.Value));

    public static IReadOnlyList<ClientRoleType> Resolve(bool isAdmin, bool isUser, IEnumerable<string> claimValues)
    {
        if (isAdmin)
            return All;

        var granted = new List<ClientRoleType>();
        foreach (var value in claimValues)
        {
            if (Enum.TryParse<ClientRoleType>(value, ignoreCase: true, out var role) &&
                All.Contains(role) &&
                !granted.Contains(role))
            {
                granted.Add(role);
            }
        }

        if (granted.Count > 0)
            return granted;

        if (isUser)
            return [ClientRoleType.Tenant];

        return [];
    }
}

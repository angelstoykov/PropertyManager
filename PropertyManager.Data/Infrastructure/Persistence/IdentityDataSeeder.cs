using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PropertyManager.Domain.Constants;
using PropertyManager.Data.Identity;

namespace PropertyManager.Data.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    public const string DefaultAdminEmail = "admin@propertymanager.local";
    public const string DefaultAdminPassword = "Admin123!";

    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var admin = await userManager.FindByEmailAsync(DefaultAdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = DefaultAdminEmail,
                Email = DefaultAdminEmail,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var result = await userManager.CreateAsync(admin, DefaultAdminPassword);
            if (!result.Succeeded)
                return;

            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        await EnsureClientAccessClaimsAsync(userManager, admin);
    }

    private static async Task EnsureClientAccessClaimsAsync(UserManager<ApplicationUser> userManager, ApplicationUser admin)
    {
        var existing = await userManager.GetClaimsAsync(admin);
        foreach (var role in ClientAccess.All)
        {
            var value = role.ToString();
            if (existing.Any(claim => claim.Type == ClientAccess.ClaimType && claim.Value == value))
                continue;

            await userManager.AddClaimAsync(admin, new Claim(ClientAccess.ClaimType, value));
        }
    }
}

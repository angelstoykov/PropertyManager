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
        if (admin is not null)
            return;

        admin = new ApplicationUser
        {
            UserName = DefaultAdminEmail,
            Email = DefaultAdminEmail,
            EmailConfirmed = true,
            FullName = "Administrator"
        };

        var result = await userManager.CreateAsync(admin, DefaultAdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}

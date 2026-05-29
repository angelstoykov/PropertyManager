using Microsoft.AspNetCore.Identity;

namespace PropertyManager.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}

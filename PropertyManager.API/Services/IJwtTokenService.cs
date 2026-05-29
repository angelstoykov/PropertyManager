using PropertyManager.Data.Identity;

namespace PropertyManager.API.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}

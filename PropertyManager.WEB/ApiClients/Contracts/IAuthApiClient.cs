using PropertyManager.Application.DTOs.Auth;

namespace PropertyManager.WEB.ApiClients.Contracts;

public interface IAuthApiClient
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<(AuthResponseDto? Response, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
}

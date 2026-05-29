using System.Net.Http.Json;
using PropertyManager.Application.DTOs.Auth;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task<(AuthResponseDto? Response, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
        if (response.IsSuccessStatusCode)
        {
            var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return (auth, []);
        }

        var body = await response.Content.ReadFromJsonAsync<RegisterErrorResponse>();
        return (null, body?.Errors ?? ["Registration failed."]);
    }

    private sealed class RegisterErrorResponse
    {
        public IEnumerable<string>? Errors { get; set; }
    }
}

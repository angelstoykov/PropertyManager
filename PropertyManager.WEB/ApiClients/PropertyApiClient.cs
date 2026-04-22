using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.Application.DTOs.Properties;

public class PropertyApiClient : IPropertyApiClient
{
    private readonly HttpClient _httpClient;

    public PropertyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PropertyListItemDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<PropertyListItemDto>>(
            "/api/properties");
    }
}

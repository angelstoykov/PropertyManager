using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.API.Controllers.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

public class PropertyApiClient : IPropertyApiClient
{
    private readonly HttpClient _httpClient;

    public PropertyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PropertyListItemDto>> GetAllAsync()
    {
        var result = await _httpClient
            .GetFromJsonAsync<IEnumerable<PropertyListItemDto>>("api/properties")
            ?? Enumerable.Empty<PropertyListItemDto>();
        
        if (!result.Any())
        {
            // log response.StatusCode and content
            return Enumerable.Empty<PropertyListItemDto>();
        }

        return result;
    }
}

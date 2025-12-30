using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Properties;

public class PropertyApiClient : IPropertyApiClient
{
    private readonly HttpClient _httpClient;

    public PropertyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PropertyListItemViewModel>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<PropertyListItemViewModel>>(
            "/api/properties");
    }
}

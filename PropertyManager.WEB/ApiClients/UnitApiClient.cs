using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Units;

public class UnitsApiClient : IUnitsApiClient
{
    private readonly HttpClient _httpClient;

    public UnitsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<UnitListViewModel>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<UnitListViewModel>>(
            "/api/units");
    }
}

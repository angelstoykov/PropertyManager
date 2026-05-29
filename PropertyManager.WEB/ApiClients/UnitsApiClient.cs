using PropertyManager.Application.DTOs.Unit;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Units;

public class UnitsApiClient : IUnitsApiClient
{
    private readonly HttpClient _httpClient;

    private const string api = "/api/units";

    public UnitsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<UnitListViewModel>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<UnitListViewModel>>(api);
    }

    public async Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query)
    {
        return await _httpClient.GetFromJsonAsync<PagedResult<UnitListItemDto>>(
            $"{api}?propertyId={query.PropertyId}&status={query.Status}&page={query.Page}&pageSize={query.PageSize}");
    }

    public async Task<UnitDto> GetUnitByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UnitDto>(
            $"{api}/{id}");
    }

    public async Task CreateUnitAsync(CreateUnitDto model)
    {
        var response = await _httpClient.PostAsJsonAsync(api, model);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUnitById(int id)
    {
        var response = await _httpClient.DeleteAsync($"{api}/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<HttpResponseMessage> UpdateUnitAsync(EditUnitDto model)
    {
        var response = await _httpClient.PutAsJsonAsync($"{api}/{model.Id}", model);
        return response.EnsureSuccessStatusCode();
    }
}

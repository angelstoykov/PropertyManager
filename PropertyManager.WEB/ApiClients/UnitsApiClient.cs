using PropertyManager.Application.DTOs.Unit;
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

    public async Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query)
    {
        return await _httpClient.GetFromJsonAsync<PagedResult<UnitListItemDto>>(
            $"/api/units?propertyId={query.PropertyId}&status={query.Status}&page={query.Page}&pageSize={query.PageSize}");
    }

    public async Task<UnitDto> GetUnitByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UnitDto>(
            $"/api/units/{id}");
    }
}

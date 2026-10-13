using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.DTOs.UnitPriceComponents;
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

    public async Task<IEnumerable<UnitListItemDto>> GetUnitsByPropertyIdAsync(int propertyId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<UnitListItemDto>>(
            $"{api}/by-property/{propertyId}");
    }

    public async Task<IReadOnlyList<UnitPriceComponentDto>> GetPriceComponentsAsync(int unitId)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<UnitPriceComponentDto>>(
            $"{api}/{unitId}/price-components");
        return result ?? Array.Empty<UnitPriceComponentDto>();
    }

    public async Task<UnitPriceComponentDto?> GetPriceComponentByIdAsync(int unitId, int id)
    {
        var response = await _httpClient.GetAsync($"{api}/{unitId}/price-components/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UnitPriceComponentDto>();
    }

    public async Task CreatePriceComponentAsync(CreateUnitPriceComponentDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{api}/{dto.UnitId}/price-components", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<HttpResponseMessage> UpdatePriceComponentAsync(int unitId, EditUnitPriceComponentDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{api}/{unitId}/price-components/{dto.Id}", dto);
        return response.EnsureSuccessStatusCode();
    }

    public async Task DeletePriceComponentAsync(int unitId, int id)
    {
        var response = await _httpClient.DeleteAsync($"{api}/{unitId}/price-components/{id}");
        response.EnsureSuccessStatusCode();
    }
}

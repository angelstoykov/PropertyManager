using Microsoft.AspNetCore.WebUtilities;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class ClientsApiClient : IClientsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string api = "/api/clients";

        public ClientsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<ClientListItemDto>> GetAllAsync(string? search = null)
        {
            var url = string.IsNullOrWhiteSpace(search)
                ? api
                : QueryHelpers.AddQueryString(api, "search", search.Trim());

            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<ClientListItemDto>>(url);
            return result ?? Array.Empty<ClientListItemDto>();
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{api}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task CreateAsync(CreateClientDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(api, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateAsync(EditClientDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{api}/{dto.Id}", dto);
            return response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{api}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyList<ClientRentedUnitDto>> GetRentedUnitsAsync(int clientId)
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<ClientRentedUnitDto>>($"{api}/{clientId}/units");
            return result ?? Array.Empty<ClientRentedUnitDto>();
        }

        public async Task<IReadOnlyList<PropertyListItemDto>> GetAvailablePropertiesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<PropertyListItemDto>>($"{api}/available-properties");
            return result ?? Array.Empty<PropertyListItemDto>();
        }

        public async Task<IReadOnlyList<UnitListItemDto>> GetAvailableUnitsByPropertyIdAsync(int propertyId)
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<UnitListItemDto>>($"{api}/available-units/{propertyId}");
            return result ?? Array.Empty<UnitListItemDto>();
        }

        public async Task AddRentedUnitAsync(int clientId, int unitId)
        {
            var response = await _httpClient.PostAsync($"{api}/{clientId}/units/{unitId}", content: null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveRentedUnitAsync(int clientId, int unitId)
        {
            var response = await _httpClient.DeleteAsync($"{api}/{clientId}/units/{unitId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyList<UnitFinancialRecordDto>> GetFinancialRecordsAsync(int clientId, int unitId)
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<UnitFinancialRecordDto>>(
                $"{api}/{clientId}/units/{unitId}/financial-records");
            return result ?? Array.Empty<UnitFinancialRecordDto>();
        }

        public async Task<UnitFinancialRecordDto?> GetFinancialRecordByIdAsync(int clientId, int unitId, int id)
        {
            var response = await _httpClient.GetAsync($"{api}/{clientId}/units/{unitId}/financial-records/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UnitFinancialRecordDto>();
        }

        public async Task CreateFinancialRecordAsync(int clientId, CreateUnitFinancialRecordDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{api}/{clientId}/units/{dto.UnitId}/financial-records", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateFinancialRecordAsync(
            int clientId,
            int unitId,
            EditUnitFinancialRecordDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"{api}/{clientId}/units/{unitId}/financial-records/{dto.Id}", dto);
            return response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFinancialRecordAsync(int clientId, int unitId, int id)
        {
            var response = await _httpClient.DeleteAsync($"{api}/{clientId}/units/{unitId}/financial-records/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

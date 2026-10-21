using System.Net;
using System.Text.Json;
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

        public async Task<ClientListResultDto> GetAllAsync(ClientListQuery? query = null)
        {
            query ??= new ClientListQuery();
            var parameters = new Dictionary<string, string?>();
            if (!string.IsNullOrWhiteSpace(query.Search))
                parameters["search"] = query.Search.Trim();
            if (!string.IsNullOrWhiteSpace(query.Tab))
                parameters["tab"] = query.Tab;
            if (query.SearchAll)
                parameters["searchAll"] = "true";
            if (!string.IsNullOrWhiteSpace(query.Status))
                parameters["status"] = query.Status;
            if (query.ClientType.HasValue)
                parameters["clientType"] = query.ClientType.Value.ToString();
            if (query.BuildingId is > 0)
                parameters["buildingId"] = query.BuildingId.Value.ToString();

            var url = parameters.Count == 0
                ? api
                : QueryHelpers.AddQueryString(api, parameters);

            var result = await _httpClient.GetFromJsonAsync<ClientListResultDto>(url);
            return result ?? new ClientListResultDto();
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
            await EnsureSuccessAsync(response);
        }

        public async Task<HttpResponseMessage> UpdateAsync(EditClientDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{api}/{dto.Id}", dto);
            await EnsureSuccessAsync(response);
            return response;
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{api}/{id}");
            await EnsureSuccessAsync(response);
        }

        public async Task DeactivateAsync(int id)
        {
            var response = await _httpClient.PostAsync($"{api}/{id}/deactivate", content: null);
            await EnsureSuccessAsync(response);
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            var message = ReadErrorMessage(body);
            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Conflict)
                throw new InvalidOperationException(message);

            throw new HttpRequestException(message);
        }

        private static string ReadErrorMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "Request failed.";

            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.String)
                {
                    return message.GetString() ?? "Request failed.";
                }
            }
            catch (JsonException)
            {
            }

            return "Request failed.";
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

        public async Task<IReadOnlyList<UnitFinancialRecordDto>> GetFinancialRecordsAsync(int clientId, int unitId, int? subtypeId = null)
        {
            var url = $"{api}/{clientId}/units/{unitId}/financial-records";
            if (subtypeId is > 0)
                url += $"?subtypeId={subtypeId.Value}";

            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<UnitFinancialRecordDto>>(url);
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

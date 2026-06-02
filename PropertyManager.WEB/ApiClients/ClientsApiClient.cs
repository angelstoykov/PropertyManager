using PropertyManager.Application.DTOs.Clients;
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

        public async Task<IReadOnlyList<ClientListItemDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<ClientListItemDto>>(api);
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
    }
}

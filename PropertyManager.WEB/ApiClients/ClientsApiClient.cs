using PropertyManager.Application.DTOs.Clients;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class ClientsApiClient : IClientsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string Api = "/api/clients";

        public ClientsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<ClientListItemDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<ClientListItemDto>>(Api);
            return result ?? Array.Empty<ClientListItemDto>();
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Api}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task CreateAsync(CreateClientDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(Api, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateAsync(EditClientDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{Api}/{dto.Id}", dto);
            return response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Api}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

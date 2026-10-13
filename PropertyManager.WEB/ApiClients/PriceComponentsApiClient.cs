using PropertyManager.Application.DTOs.PriceComponents;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class PriceComponentsApiClient : IPriceComponentsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string api = "/api/price-components";

        public PriceComponentsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<PriceComponentDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<PriceComponentDto>>(api);
            return result ?? Array.Empty<PriceComponentDto>();
        }

        public async Task<PriceComponentDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{api}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PriceComponentDto>();
        }

        public async Task CreateAsync(CreatePriceComponentDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(api, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateAsync(EditPriceComponentDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{api}/{dto.Id}", dto);
            return response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{api}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

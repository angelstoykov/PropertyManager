using PropertyManager.Application.DTOs.FinancialRecordSubtypes;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class FinancialRecordSubtypesApiClient : IFinancialRecordSubtypesApiClient
    {
        private readonly HttpClient _httpClient;
        private const string api = "/api/financial-record-subtypes";

        public FinancialRecordSubtypesApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<FinancialRecordSubtypeDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<FinancialRecordSubtypeDto>>(api);
            return result ?? Array.Empty<FinancialRecordSubtypeDto>();
        }

        public async Task<FinancialRecordSubtypeDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{api}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FinancialRecordSubtypeDto>();
        }

        public async Task CreateAsync(CreateFinancialRecordSubtypeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(api, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateAsync(EditFinancialRecordSubtypeDto dto)
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

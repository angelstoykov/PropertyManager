using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class FinancialRecordsApiClient : IFinancialRecordsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string api = "/api/financial-records";

        public FinancialRecordsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<UnitFinancialRecordDto>> GetFinancialRecordsAsync(int clientId, int unitId, int? subtypeId = null)
        {
            var url = $"{api}/clients/{clientId}/units/{unitId}";
            if (subtypeId is > 0)
                url += $"?subtypeId={subtypeId.Value}";

            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<UnitFinancialRecordDto>>(url);
            return result ?? Array.Empty<UnitFinancialRecordDto>();
        }

        public async Task<UnitFinancialRecordDto?> GetFinancialRecordByIdAsync(int clientId, int unitId, int id)
        {
            var response = await _httpClient.GetAsync($"{api}/clients/{clientId}/units/{unitId}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UnitFinancialRecordDto>();
        }

        public async Task CreateFinancialRecordAsync(int clientId, CreateUnitFinancialRecordDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{api}/clients/{clientId}/units/{dto.UnitId}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateFinancialRecordAsync(
            int clientId,
            int unitId,
            EditUnitFinancialRecordDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"{api}/clients/{clientId}/units/{unitId}/{dto.Id}", dto);
            return response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFinancialRecordAsync(int clientId, int unitId, int id)
        {
            var response = await _httpClient.DeleteAsync($"{api}/clients/{clientId}/units/{unitId}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

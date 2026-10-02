using PropertyManager.Application.DTOs.Currencies;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class CurrencyApiClient : ICurrencyApiClient
    {
        private readonly HttpClient _httpClient;

        public CurrencyApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<IEnumerable<CurrencyDto>>("api/currencies")
                ?? Enumerable.Empty<CurrencyDto>();

            if (!result.Any())
            {
                // log response.StatusCode and content
                return Enumerable.Empty<CurrencyDto>();
            }

            return result;
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllActiveAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<IEnumerable<CurrencyDto>>("api/currencies/active")
                ?? Enumerable.Empty<CurrencyDto>();

            if (!result.Any())
            {
                // log response.StatusCode and content
                return Enumerable.Empty<CurrencyDto>();
            }

            return result;
        }
    }
}
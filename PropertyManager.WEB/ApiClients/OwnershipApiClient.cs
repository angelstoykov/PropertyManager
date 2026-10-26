using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using PropertyManager.Application.DTOs.Ownership;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients
{
    public class OwnershipApiClient : IOwnershipApiClient
    {
        private readonly HttpClient _httpClient;
        private const string Api = "/api/ownership";

        public OwnershipApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TargetOwnershipDto> GetPropertyAsync(int propertyId, DateTime? date)
        {
            var response = await _httpClient.GetAsync(WithDate($"{Api}/properties/{propertyId}", date));
            await EnsureSuccessAsync(response);
            return (await response.Content.ReadFromJsonAsync<TargetOwnershipDto>())!;
        }

        public async Task SetPropertyAsync(int propertyId, SetOwnershipDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{Api}/properties/{propertyId}", dto);
            await EnsureSuccessAsync(response);
        }

        public async Task<TargetOwnershipDto> GetUnitAsync(int unitId, DateTime? date)
        {
            var response = await _httpClient.GetAsync(WithDate($"{Api}/units/{unitId}", date));
            await EnsureSuccessAsync(response);
            return (await response.Content.ReadFromJsonAsync<TargetOwnershipDto>())!;
        }

        public async Task SetUnitAsync(int unitId, SetOwnershipDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{Api}/units/{unitId}", dto);
            await EnsureSuccessAsync(response);
        }

        public async Task<ClientOwnershipDto> GetClientAsync(int clientId)
        {
            var response = await _httpClient.GetAsync($"{Api}/clients/{clientId}");
            await EnsureSuccessAsync(response);
            return (await response.Content.ReadFromJsonAsync<ClientOwnershipDto>())!;
        }

        private static string WithDate(string url, DateTime? date) =>
            date == null ? url : QueryHelpers.AddQueryString(url, "date", date.Value.ToString("yyyy-MM-dd"));

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            var message = "Request failed.";
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var document = JsonDocument.Parse(body);
                    if (document.RootElement.TryGetProperty("message", out var value) &&
                        value.ValueKind == JsonValueKind.String)
                    {
                        message = value.GetString() ?? message;
                    }
                }
                catch (JsonException)
                {
                    message = body;
                }
            }

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Forbidden)
                throw new InvalidOperationException(message == "Request failed." && response.StatusCode == HttpStatusCode.Forbidden
                    ? "You do not have access to ownership."
                    : message);

            throw new HttpRequestException(message);
        }
    }
}

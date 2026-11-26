using System.Net;
using System.Net.Http.Json;
using PropertyManager.Reporting;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ApiClients;

public class ReportsApiClient : IReportsApiClient
{
    private readonly HttpClient _httpClient;

    public ReportsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ReportCatalogItem>> GetCatalogAsync()
    {
        var catalog = await _httpClient.GetFromJsonAsync<List<ReportCatalogItem>>("reports");
        return catalog ?? new List<ReportCatalogItem>();
    }

    public async Task<ReportCallResult> RunAsync(string key, ReportRunRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"reports/{Uri.EscapeDataString(key)}/run", request);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadFromJsonAsync<ReportErrorResponse>();
            return new ReportCallResult
            {
                Errors = body?.Errors ?? new List<ReportFieldError>()
            };
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new ReportCallResult
            {
                Errors = new[]
                {
                    new ReportFieldError { Parameter = "reportKey", Message = "Report was not found." }
                }
            };
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReportRunResult>();
        return new ReportCallResult { Result = result };
    }
}

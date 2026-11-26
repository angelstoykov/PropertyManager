using System.Net.Http.Json;
using System.Text.Json;
using PropertyManager.Reporting;

namespace PropertyManager.API.Reporting;

public class HttpReportDataGateway : IReportDataGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpReportDataGateway(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<AccessibleBuilding>> GetBuildingsAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var properties = await client.GetFromJsonAsync<List<PropertyLine>>("api/properties", JsonOptions, cancellationToken)
            ?? new List<PropertyLine>();

        return properties
            .Select(property => new AccessibleBuilding
            {
                Id = property.Id,
                Name = property.Name ?? ""
            })
            .ToList();
    }

    public async Task<IReadOnlyList<BuildingCurrencyTotal>> GetBuildingTotalsAsync(
        DateTime startDate,
        DateTime endDate,
        IReadOnlyCollection<int> propertyIds,
        string? type,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var query = new List<string>
        {
            $"startDate={startDate:yyyy-MM-dd}",
            $"endDate={endDate:yyyy-MM-dd}"
        };

        foreach (var id in propertyIds.Distinct())
            query.Add($"propertyIds={id}");

        if (!string.IsNullOrWhiteSpace(type))
            query.Add($"type={Uri.EscapeDataString(type)}");

        var totals = await client.GetFromJsonAsync<List<TotalLine>>(
            "api/financial-records/totals?" + string.Join("&", query),
            JsonOptions,
            cancellationToken) ?? new List<TotalLine>();

        return totals.Select(total => new BuildingCurrencyTotal
        {
            PropertyId = total.PropertyId,
            CurrencyCode = total.CurrencyCode ?? "",
            CurrencySymbol = string.IsNullOrEmpty(total.CurrencySymbol) ? total.CurrencyCode ?? "" : total.CurrencySymbol,
            Income = total.Income,
            Expense = total.Expense
        }).ToList();
    }

    private HttpClient CreateClient()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("Reporting data is only available during an HTTP request.");

        var request = httpContext.Request;
        var client = _httpClientFactory.CreateClient(nameof(HttpReportDataGateway));
        var pathBase = request.PathBase.HasValue ? request.PathBase.Value : "";
        client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}{pathBase}/");

        var authorization = request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

        return client;
    }

    private sealed class PropertyLine
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class TotalLine
    {
        public int PropertyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
}

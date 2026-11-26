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

    public async Task<IReadOnlyList<UnitIncomeTotal>> GetUnitIncomeAsync(
        DateTime startDate,
        DateTime endDate,
        IReadOnlyCollection<int> propertyIds,
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

        var totals = await client.GetFromJsonAsync<List<UnitIncomeLine>>(
            "api/financial-records/unit-income?" + string.Join("&", query),
            JsonOptions,
            cancellationToken) ?? new List<UnitIncomeLine>();

        return totals.Select(line => new UnitIncomeTotal
        {
            PropertyId = line.PropertyId,
            UnitId = line.UnitId,
            UnitNumber = line.UnitNumber ?? "",
            CurrencyCode = line.CurrencyCode ?? "",
            CurrencySymbol = string.IsNullOrEmpty(line.CurrencySymbol) ? line.CurrencyCode ?? "" : line.CurrencySymbol,
            Amount = line.Amount,
            IncomeCount = line.IncomeCount
        }).ToList();
    }

    public async Task<IReadOnlyList<UnitCommissionSetup>> GetUnitCommissionSetupAsync(
        IReadOnlyCollection<int> propertyIds,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var query = propertyIds.Distinct().Select(id => $"propertyIds={id}");
        var url = "api/unit-price-setup";
        var joined = string.Join("&", query);
        if (joined.Length > 0)
            url += "?" + joined;

        var lines = await client.GetFromJsonAsync<List<SetupLine>>(url, JsonOptions, cancellationToken)
            ?? new List<SetupLine>();

        return lines.Select(line => new UnitCommissionSetup
        {
            UnitId = line.UnitId,
            CurrencyCode = line.CurrencyCode ?? "",
            IncludedAmount = line.IncludedAmount,
            ExcludedAmount = line.ExcludedAmount
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

    private sealed class UnitIncomeLine
    {
        public int PropertyId { get; set; }
        public int UnitId { get; set; }
        public string? UnitNumber { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public int IncomeCount { get; set; }
    }

    private sealed class SetupLine
    {
        public int UnitId { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal IncludedAmount { get; set; }
        public decimal ExcludedAmount { get; set; }
    }
}

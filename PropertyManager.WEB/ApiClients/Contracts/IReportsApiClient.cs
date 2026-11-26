using PropertyManager.Reporting;

namespace PropertyManager.WEB.ApiClients.Contracts;

public interface IReportsApiClient
{
    Task<IReadOnlyList<ReportCatalogItem>> GetCatalogAsync();
    Task<ReportCallResult> RunAsync(string key, ReportRunRequest request);
}

public class ReportCallResult
{
    public ReportRunResult? Result { get; init; }
    public IReadOnlyList<ReportFieldError> Errors { get; init; } = Array.Empty<ReportFieldError>();
    public bool IsSuccess => Result != null;
}

namespace PropertyManager.Reporting;

public class AccessibleBuilding
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class BuildingCurrencyTotal
{
    public int PropertyId { get; set; }
    public string CurrencyCode { get; set; } = "";
    public string CurrencySymbol { get; set; } = "";
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}

public interface IReportDataGateway
{
    Task<IReadOnlyList<AccessibleBuilding>> GetBuildingsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BuildingCurrencyTotal>> GetBuildingTotalsAsync(
        DateTime startDate,
        DateTime endDate,
        IReadOnlyCollection<int> propertyIds,
        string? type,
        CancellationToken cancellationToken = default);
}

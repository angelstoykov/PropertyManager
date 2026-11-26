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

public class UnitIncomeTotal
{
    public int PropertyId { get; set; }
    public int UnitId { get; set; }
    public string UnitNumber { get; set; } = "";
    public string CurrencyCode { get; set; } = "";
    public string CurrencySymbol { get; set; } = "";
    public decimal Amount { get; set; }
    public int IncomeCount { get; set; }
}

public class UnitCommissionSetup
{
    public int UnitId { get; set; }
    public string CurrencyCode { get; set; } = "";
    public decimal IncludedAmount { get; set; }
    public decimal ExcludedAmount { get; set; }
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

    Task<IReadOnlyList<UnitIncomeTotal>> GetUnitIncomeAsync(
        DateTime startDate,
        DateTime endDate,
        IReadOnlyCollection<int> propertyIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnitCommissionSetup>> GetUnitCommissionSetupAsync(
        IReadOnlyCollection<int> propertyIds,
        CancellationToken cancellationToken = default);
}

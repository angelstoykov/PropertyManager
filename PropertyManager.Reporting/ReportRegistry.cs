namespace PropertyManager.Reporting;

public class ReportRegistry
{
    private readonly IReadOnlyList<ReportDefinition> _reports;

    public ReportRegistry()
    {
        _reports = new[] { IncomeAndExpensesByBuilding(), InvoicingRevenueForPeriod() };
    }

    public IReadOnlyList<ReportDefinition> All => _reports;

    public ReportDefinition? Find(string key) =>
        _reports.FirstOrDefault(report => string.Equals(report.Key, key, StringComparison.OrdinalIgnoreCase));

    private static ReportDefinition IncomeAndExpensesByBuilding()
    {
        return new ReportDefinition
        {
            Key = "income-expenses-by-building",
            Name = "Income and expenses by building",
            Category = "Finance",
            DataSourceKey = ReportDataSources.FinanceBuildingTotals,
            GroupingOptions = new List<string> { "building" },
            Parameters = new List<ReportParameterDefinition>
            {
                new()
                {
                    Key = ReportParameterKeys.Period,
                    Label = "Period",
                    Kind = "dateRange",
                    Scope = "common",
                    Required = true
                },
                new()
                {
                    Key = ReportParameterKeys.Buildings,
                    Label = "Buildings",
                    Kind = "multiSelect",
                    Scope = "common",
                    OptionSource = "buildings"
                },
                new()
                {
                    Key = ReportParameterKeys.Type,
                    Label = "Type",
                    Kind = "choice",
                    Scope = "specific",
                    DefaultValue = "Both",
                    Options = new List<ReportParameterOption>
                    {
                        new() { Value = "Both", Label = "Income and expenses" },
                        new() { Value = "Income", Label = "Income" },
                        new() { Value = "Expense", Label = "Expense" }
                    }
                },
                new()
                {
                    Key = ReportParameterKeys.Grouping,
                    Label = "Grouping",
                    Kind = "choice",
                    Scope = "specific",
                    DefaultValue = "building",
                    Options = new List<ReportParameterOption>
                    {
                        new() { Value = "building", Label = "Building" }
                    }
                }
            },
            Columns = new List<ReportColumnDefinition>
            {
                new() { Key = "building", Label = "Building", Field = "buildingName" },
                new()
                {
                    Key = "income",
                    Label = "Income",
                    Field = "income",
                    Numeric = true,
                    VisibleWhenParameter = ReportParameterKeys.Type,
                    VisibleWhenValues = new List<string> { "Income", "Both" }
                },
                new()
                {
                    Key = "expense",
                    Label = "Expense",
                    Field = "expense",
                    Numeric = true,
                    VisibleWhenParameter = ReportParameterKeys.Type,
                    VisibleWhenValues = new List<string> { "Expense", "Both" }
                },
                new()
                {
                    Key = "net",
                    Label = "Net",
                    Formula = "income-expense",
                    Numeric = true,
                    VisibleWhenParameter = ReportParameterKeys.Type,
                    VisibleWhenValues = new List<string> { "Both" }
                }
            }
        };
    }

    private static ReportDefinition InvoicingRevenueForPeriod()
    {
        return new ReportDefinition
        {
            Key = "invoicing-revenue-for-a-period",
            Name = "Invoicing revenue for a period",
            Category = "Finance",
            DataSourceKey = ReportDataSources.InvoicingRevenue,
            Parameters = new List<ReportParameterDefinition>
            {
                new()
                {
                    Key = ReportParameterKeys.Period,
                    Label = "Period",
                    Kind = "dateRange",
                    Scope = "common",
                    Required = true
                },
                new()
                {
                    Key = ReportParameterKeys.Buildings,
                    Label = "Buildings",
                    Kind = "multiSelect",
                    Scope = "common",
                    OptionSource = "buildings"
                }
            },
            Columns = new List<ReportColumnDefinition>
            {
                new() { Key = "building", Label = "Building", Field = "buildingName" },
                new() { Key = "unit", Label = "Unit", Field = "unitName" },
                new() { Key = "received", Label = "Received", Field = "received", Numeric = true },
                new() { Key = "invoiceable", Label = "Invoiceable", Field = "invoiceable", Numeric = true }
            }
        };
    }
}

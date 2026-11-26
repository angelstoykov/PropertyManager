namespace PropertyManager.Reporting;

public static class ReportParameterKeys
{
    public const string Period = "period";
    public const string Buildings = "buildings";
    public const string Type = "type";
    public const string Grouping = "grouping";
}

public static class ReportDataSources
{
    public const string FinanceBuildingTotals = "finance.building-totals";
    public const string InvoicingRevenue = "finance.unit-invoicing";
}

public class ReportParameterOption
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
}

public class ReportParameterDefinition
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Scope { get; set; } = "";
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionSource { get; set; }
    public List<ReportParameterOption> Options { get; set; } = new();
}

public class ReportColumnDefinition
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Field { get; set; }
    public string? Formula { get; set; }
    public bool Numeric { get; set; }
    public string? VisibleWhenParameter { get; set; }
    public List<string> VisibleWhenValues { get; set; } = new();
}

public class ReportDefinition
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    public string DataSourceKey { get; set; } = "";
    public List<string> GroupingOptions { get; set; } = new();
    public List<ReportParameterDefinition> Parameters { get; set; } = new();
    public List<ReportColumnDefinition> Columns { get; set; } = new();

    public bool Uses(string parameterKey) =>
        Parameters.Any(parameter => string.Equals(parameter.Key, parameterKey, StringComparison.OrdinalIgnoreCase));
}

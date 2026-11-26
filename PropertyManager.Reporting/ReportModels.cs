namespace PropertyManager.Reporting;

public class ReportCatalogItem
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public List<ReportParameterDefinition> CommonParameters { get; set; } = new();
    public List<ReportParameterDefinition> Parameters { get; set; } = new();
}

public class ReportRunRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? BuildingIds { get; set; }
    public Dictionary<string, string?>? Parameters { get; set; }
}

public class ReportFieldError
{
    public string Parameter { get; set; } = "";
    public string Message { get; set; } = "";
}

public class ReportErrorResponse
{
    public List<ReportFieldError> Errors { get; set; } = new();
}

public class ReportHeaderItem
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
}

public class ReportColumn
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public bool Numeric { get; set; }
}

public class ReportValue
{
    public string? Text { get; set; }
    public decimal? Amount { get; set; }
}

public class ReportTable
{
    public string Title { get; set; } = "";
    public List<ReportColumn> Columns { get; set; } = new();
    public List<Dictionary<string, ReportValue>> Rows { get; set; } = new();
    public Dictionary<string, ReportValue> Totals { get; set; } = new();
}

public class ReportRunResult
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public List<ReportHeaderItem> Header { get; set; } = new();
    public List<ReportTable> Tables { get; set; } = new();
    public bool HasData { get; set; }
    public string? EmptyMessage { get; set; }
    public string AmountBasis { get; set; } =
        "Amounts are the recorded financial-record amounts in each currency. Financial records do not store VAT separately, so these totals are not adjusted for VAT. Different currencies are totaled separately and are not converted.";
}

public class ReportNotFoundException : Exception
{
    public ReportNotFoundException(string message) : base(message)
    {
    }
}

public class ReportValidationException : Exception
{
    public ReportValidationException(List<ReportFieldError> errors)
        : base("The report request is not valid.")
    {
        Errors = errors;
    }

    public List<ReportFieldError> Errors { get; }
}

using PropertyManager.Reporting;

namespace PropertyManager.WEB.ViewModels.Reports;

public class BuildingChoice
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class CommonParameterBlock
{
    public ReportParameterDefinition Definition { get; set; } = new();
    public List<string> ReportKeys { get; set; } = new();
}

public class ReportsPageForm
{
    public string? ReportKey { get; set; }
    public string? PeriodPreset { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? BuildingIds { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}

public class ReportParameterViewModel
{
    public string? ReportKey { get; set; }
    public ReportParameterDefinition Definition { get; set; } = new();
    public bool Disabled { get; set; }
    public string PeriodPreset { get; set; } = "thisMonth";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> BuildingIds { get; set; } = new();
    public List<BuildingChoice> Buildings { get; set; } = new();
    public string? ChoiceValue { get; set; }
    public string? Error { get; set; }
    public string? StartError { get; set; }
    public string? EndError { get; set; }
}

public class ReportsPageViewModel
{
    public List<ReportCatalogItem> Catalog { get; set; } = new();
    public List<BuildingChoice> Buildings { get; set; } = new();
    public List<CommonParameterBlock> CommonParameters { get; set; } = new();
    public string? SelectedKey { get; set; }
    public string PeriodPreset { get; set; } = "thisMonth";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> BuildingIds { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Errors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public ReportRunResult? Result { get; set; }
    public bool Generated { get; set; }

    public ReportParameterViewModel Field(ReportParameterDefinition definition, string? reportKey, bool disabled)
    {
        Parameters.TryGetValue(definition.Key, out var choice);
        Errors.TryGetValue(definition.Key, out var error);
        Errors.TryGetValue("startDate", out var startError);
        Errors.TryGetValue("endDate", out var endError);
        Errors.TryGetValue("buildingIds", out var buildingError);

        return new ReportParameterViewModel
        {
            ReportKey = reportKey,
            Definition = definition,
            Disabled = disabled,
            PeriodPreset = PeriodPreset,
            StartDate = StartDate,
            EndDate = EndDate,
            BuildingIds = BuildingIds,
            Buildings = Buildings,
            ChoiceValue = string.IsNullOrWhiteSpace(choice) ? definition.DefaultValue : choice,
            Error = definition.Kind == "multiSelect" ? buildingError : error,
            StartError = startError,
            EndError = endError
        };
    }

    public IEnumerable<string> ExtraErrors(ReportCatalogItem report)
    {
        var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "startDate",
            "endDate",
            "buildingIds",
            "reportKey"
        };

        foreach (var parameter in report.Parameters)
            known.Add(parameter.Key);
        foreach (var parameter in report.CommonParameters)
            known.Add(parameter.Key);

        return Errors.Where(pair => !known.Contains(pair.Key)).Select(pair => pair.Value);
    }
}

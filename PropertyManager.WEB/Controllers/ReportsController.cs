using Microsoft.AspNetCore.Mvc;
using PropertyManager.Reporting;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Reports;

namespace PropertyManager.WEB.Controllers;

public class ReportsController : Controller
{
    private readonly IReportsApiClient _reportsApiClient;
    private readonly IPropertyApiClient _propertyApiClient;

    public ReportsController(IReportsApiClient reportsApiClient, IPropertyApiClient propertyApiClient)
    {
        _reportsApiClient = reportsApiClient;
        _propertyApiClient = propertyApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var (catalog, buildings) = await LoadLookupsAsync();
        var (start, end) = ThisMonth();
        return View(BuildPage(catalog, buildings, new ReportsPageForm
        {
            ReportKey = catalog.FirstOrDefault()?.Key,
            PeriodPreset = "thisMonth",
            StartDate = start,
            EndDate = end
        }, null, null, false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ReportsPageForm form)
    {
        var (catalog, buildings) = await LoadLookupsAsync();
        form.BuildingIds ??= new List<int>();
        form.Parameters ??= new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(form.ReportKey))
        {
            var missing = BuildPage(catalog, buildings, form, null, null, false);
            missing.Errors["reportKey"] = "Choose a report.";
            return View(missing);
        }

        var call = await _reportsApiClient.RunAsync(form.ReportKey, new ReportRunRequest
        {
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            BuildingIds = form.BuildingIds,
            Parameters = form.Parameters.ToDictionary(pair => pair.Key, pair => (string?)pair.Value)
        });

        return View(BuildPage(catalog, buildings, form, call.Result, call.Errors, call.IsSuccess));
    }

    private async Task<(List<ReportCatalogItem> Catalog, List<BuildingChoice> Buildings)> LoadLookupsAsync()
    {
        var catalog = (await _reportsApiClient.GetCatalogAsync()).ToList();
        var properties = await _propertyApiClient.GetAllAsync();
        var buildings = properties
            .Select(property => new BuildingChoice { Id = property.Id, Name = property.Name })
            .OrderBy(building => building.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(building => building.Id)
            .ToList();
        return (catalog, buildings);
    }

    private static ReportsPageViewModel BuildPage(
        List<ReportCatalogItem> catalog,
        List<BuildingChoice> buildings,
        ReportsPageForm form,
        ReportRunResult? result,
        IReadOnlyList<ReportFieldError>? errors,
        bool generated)
    {
        var page = new ReportsPageViewModel
        {
            Catalog = catalog,
            Buildings = buildings,
            CommonParameters = CommonParameters(catalog),
            SelectedKey = string.IsNullOrWhiteSpace(form.ReportKey) ? catalog.FirstOrDefault()?.Key : form.ReportKey,
            PeriodPreset = string.IsNullOrWhiteSpace(form.PeriodPreset) ? "thisMonth" : form.PeriodPreset,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            BuildingIds = form.BuildingIds ?? new List<int>(),
            Parameters = new Dictionary<string, string>(form.Parameters ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase),
            Result = result,
            Generated = generated
        };

        if (errors != null)
        {
            foreach (var error in errors)
                page.Errors[error.Parameter] = error.Message;
        }

        return page;
    }

    private static List<CommonParameterBlock> CommonParameters(IReadOnlyList<ReportCatalogItem> catalog)
    {
        var blocks = new List<CommonParameterBlock>();
        foreach (var report in catalog)
        {
            foreach (var parameter in report.CommonParameters)
            {
                var existing = blocks.FirstOrDefault(block =>
                    string.Equals(block.Definition.Key, parameter.Key, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    blocks.Add(new CommonParameterBlock
                    {
                        Definition = parameter,
                        ReportKeys = new List<string> { report.Key }
                    });
                }
                else if (!existing.ReportKeys.Contains(report.Key, StringComparer.OrdinalIgnoreCase))
                {
                    existing.ReportKeys.Add(report.Key);
                }
            }
        }

        return blocks;
    }

    private static (DateTime Start, DateTime End) ThisMonth()
    {
        var today = DateTime.Today;
        var start = new DateTime(today.Year, today.Month, 1);
        return (start, start.AddMonths(1).AddDays(-1));
    }
}

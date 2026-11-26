using System.Globalization;

namespace PropertyManager.Reporting;

public class ReportRunner
{
    private static readonly CultureInfo EnglishDates = CultureInfo.GetCultureInfo("en-GB");

    private readonly ReportRegistry _registry;
    private readonly IReportDataGateway _gateway;

    public ReportRunner(ReportRegistry registry, IReportDataGateway gateway)
    {
        _registry = registry;
        _gateway = gateway;
    }

    public IReadOnlyList<ReportCatalogItem> ListForUser(IEnumerable<string> roles)
    {
        var roleSet = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
        return _registry.All
            .Where(report => report.Roles.Count == 0 || report.Roles.Any(roleSet.Contains))
            .Select(ToCatalogItem)
            .ToList();
    }

    public async Task<ReportRunResult> RunAsync(
        string key,
        ReportRunRequest request,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        var roleSet = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
        var report = _registry.Find(key);
        if (report == null || (report.Roles.Count > 0 && !report.Roles.Any(roleSet.Contains)))
            throw new ReportNotFoundException("Report was not found.");

        var errors = new List<ReportFieldError>();
        var parameters = ResolveParameters(report, request, errors);
        var buildings = await ResolveBuildingsAsync(request, errors, cancellationToken);
        if (errors.Count > 0 || request.StartDate == null || request.EndDate == null)
            throw new ReportValidationException(errors);

        var facts = await LoadFactsAsync(
            report,
            parameters,
            buildings,
            request.StartDate.Value,
            request.EndDate.Value,
            cancellationToken);
        return BuildResult(report, request, parameters, buildings, facts);
    }

    private static ReportCatalogItem ToCatalogItem(ReportDefinition report) => new()
    {
        Key = report.Key,
        Name = report.Name,
        Category = report.Category,
        CommonParameters = report.Parameters.Where(parameter => parameter.Scope == "common").ToList(),
        Parameters = report.Parameters.Where(parameter => parameter.Scope == "specific").ToList()
    };

    private static Dictionary<string, string> ResolveParameters(
        ReportDefinition report,
        ReportRunRequest request,
        List<ReportFieldError> errors)
    {
        var resolved = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (request.StartDate == null)
            errors.Add(new ReportFieldError { Parameter = "startDate", Message = "Start date is required." });
        if (request.EndDate == null)
            errors.Add(new ReportFieldError { Parameter = "endDate", Message = "End date is required." });
        if (request.StartDate != null && request.EndDate != null && request.EndDate.Value.Date < request.StartDate.Value.Date)
            errors.Add(new ReportFieldError { Parameter = "endDate", Message = "End date cannot be before start date." });

        var supplied = new Dictionary<string, string?>(request.Parameters ?? new Dictionary<string, string?>(), StringComparer.OrdinalIgnoreCase);
        var specific = report.Parameters.Where(parameter => parameter.Scope == "specific").ToList();
        foreach (var pair in supplied)
        {
            if (specific.All(parameter => !string.Equals(parameter.Key, pair.Key, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(new ReportFieldError
                {
                    Parameter = pair.Key,
                    Message = $"Parameter '{pair.Key}' is not used by this report."
                });
            }
        }

        foreach (var parameter in specific)
        {
            supplied.TryGetValue(parameter.Key, out var raw);
            var value = string.IsNullOrWhiteSpace(raw) ? parameter.DefaultValue : raw.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                if (parameter.Required)
                {
                    errors.Add(new ReportFieldError
                    {
                        Parameter = parameter.Key,
                        Message = $"{parameter.Label} is required."
                    });
                }

                continue;
            }

            if (parameter.Kind == "choice" && parameter.Options.Count > 0)
            {
                var option = parameter.Options.FirstOrDefault(candidate =>
                    string.Equals(candidate.Value, value, StringComparison.OrdinalIgnoreCase));
                if (option == null)
                {
                    errors.Add(new ReportFieldError
                    {
                        Parameter = parameter.Key,
                        Message = $"{parameter.Label} is not an allowed value."
                    });
                    continue;
                }

                value = option.Value;
            }

            resolved[parameter.Key] = value;
        }

        return resolved;
    }

    private async Task<List<AccessibleBuilding>> ResolveBuildingsAsync(
        ReportRunRequest request,
        List<ReportFieldError> errors,
        CancellationToken cancellationToken)
    {
        var accessible = await _gateway.GetBuildingsAsync(cancellationToken);
        var requested = request.BuildingIds?
            .Where(id => id > 0)
            .Distinct()
            .ToList() ?? new List<int>();

        if (requested.Count == 0)
        {
            return accessible
                .OrderBy(building => building.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(building => building.Id)
                .ToList();
        }

        var byId = accessible.ToDictionary(building => building.Id);
        if (requested.Any(id => !byId.ContainsKey(id)))
        {
            errors.Add(new ReportFieldError
            {
                Parameter = "buildingIds",
                Message = "You do not have access to one or more selected buildings."
            });
            return new List<AccessibleBuilding>();
        }

        return requested
            .Select(id => byId[id])
            .OrderBy(building => building.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(building => building.Id)
            .ToList();
    }

    private async Task<IReadOnlyList<BuildingCurrencyTotal>> LoadFactsAsync(
        ReportDefinition report,
        IReadOnlyDictionary<string, string> parameters,
        IReadOnlyList<AccessibleBuilding> buildings,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(report.DataSourceKey, ReportDataSources.FinanceBuildingTotals, StringComparison.Ordinal))
            throw new InvalidOperationException($"Unknown data source '{report.DataSourceKey}'.");

        if (buildings.Count == 0)
            return Array.Empty<BuildingCurrencyTotal>();

        parameters.TryGetValue(ReportParameterKeys.Type, out var type);
        var financeType = string.IsNullOrWhiteSpace(type) || string.Equals(type, "Both", StringComparison.OrdinalIgnoreCase)
            ? null
            : type;

        return await _gateway.GetBuildingTotalsAsync(
            startDate,
            endDate,
            buildings.Select(building => building.Id).ToList(),
            financeType,
            cancellationToken);
    }

    private static ReportRunResult BuildResult(
        ReportDefinition report,
        ReportRunRequest request,
        IReadOnlyDictionary<string, string> parameters,
        IReadOnlyList<AccessibleBuilding> buildings,
        IReadOnlyList<BuildingCurrencyTotal> facts)
    {
        var hasData = facts.Any(fact => fact.Income != 0 || fact.Expense != 0);
        var result = new ReportRunResult
        {
            Key = report.Key,
            Name = report.Name,
            Header = BuildHeader(report, request, parameters, buildings),
            HasData = hasData,
            EmptyMessage = hasData ? null : "No financial records match this period and building selection."
        };

        if (!hasData)
            return result;

        var columns = VisibleColumns(report, parameters);
        var factsByCurrency = facts
            .GroupBy(fact => fact.CurrencyCode, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var currency in factsByCurrency)
        {
            var sample = currency.First();
            var byBuilding = currency.ToDictionary(fact => fact.PropertyId);
            var rows = new List<Dictionary<string, ReportValue>>();
            decimal incomeTotal = 0;
            decimal expenseTotal = 0;

            foreach (var building in buildings)
            {
                byBuilding.TryGetValue(building.Id, out var fact);
                var income = fact?.Income ?? 0;
                var expense = fact?.Expense ?? 0;
                incomeTotal += income;
                expenseTotal += expense;
                rows.Add(BuildRow(columns, building.Name, income, expense));
            }

            result.Tables.Add(new ReportTable
            {
                Title = CurrencyTitle(sample.CurrencyCode, sample.CurrencySymbol),
                Columns = columns.Select(column => new ReportColumn
                {
                    Key = column.Key,
                    Label = column.Label,
                    Numeric = column.Numeric
                }).ToList(),
                Rows = rows,
                Totals = BuildRow(columns, "Total", incomeTotal, expenseTotal)
            });
        }

        return result;
    }

    private static List<ReportHeaderItem> BuildHeader(
        ReportDefinition report,
        ReportRunRequest request,
        IReadOnlyDictionary<string, string> parameters,
        IReadOnlyList<AccessibleBuilding> buildings)
    {
        var unrestricted = request.BuildingIds == null || !request.BuildingIds.Any(id => id > 0);
        var buildingValue = unrestricted
            ? "All buildings"
            : string.Join(", ", buildings.Select(building => building.Name));

        var header = new List<ReportHeaderItem>
        {
            new()
            {
                Label = "Period",
                Value = $"{request.StartDate?.ToString("dd MMM yyyy", EnglishDates)} – {request.EndDate?.ToString("dd MMM yyyy", EnglishDates)} (inclusive)"
            },
            new() { Label = "Buildings", Value = buildingValue }
        };

        foreach (var parameter in report.Parameters.Where(parameter => parameter.Scope == "specific"))
        {
            if (!parameters.TryGetValue(parameter.Key, out var value))
                continue;

            var label = parameter.Options
                .FirstOrDefault(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
                ?.Label ?? value;
            header.Add(new ReportHeaderItem { Label = parameter.Label, Value = label });
        }

        return header;
    }

    private static List<ReportColumnDefinition> VisibleColumns(
        ReportDefinition report,
        IReadOnlyDictionary<string, string> parameters)
    {
        return report.Columns.Where(column =>
        {
            if (string.IsNullOrEmpty(column.VisibleWhenParameter))
                return true;

            if (!parameters.TryGetValue(column.VisibleWhenParameter, out var value))
                return false;

            return column.VisibleWhenValues.Any(allowed =>
                string.Equals(allowed, value, StringComparison.OrdinalIgnoreCase));
        }).ToList();
    }

    private static Dictionary<string, ReportValue> BuildRow(
        IReadOnlyList<ReportColumnDefinition> columns,
        string buildingName,
        decimal income,
        decimal expense)
    {
        var row = new Dictionary<string, ReportValue>(StringComparer.OrdinalIgnoreCase);
        foreach (var column in columns)
        {
            if (!column.Numeric)
            {
                row[column.Key] = new ReportValue { Text = buildingName };
                continue;
            }

            decimal amount = column.Formula switch
            {
                "income-expense" => income - expense,
                _ when column.Field == "income" => income,
                _ when column.Field == "expense" => expense,
                _ => 0
            };
            row[column.Key] = new ReportValue { Amount = amount };
        }

        return row;
    }

    private static string CurrencyTitle(string code, string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol) || string.Equals(symbol, code, StringComparison.OrdinalIgnoreCase))
            return code;
        return $"{code} ({symbol})";
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.DTOs.UnitPriceComponents;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Units;

public class UnitsController : Controller
{
    private readonly IPropertyApiClient _propertyApiClient;
    private readonly IUnitsApiClient _unitsApiClient;
    private readonly ICurrencyApiClient _currencyApiClient;
    private readonly IPriceComponentsApiClient _priceComponentsApiClient;

    public UnitsController(
        IPropertyApiClient propertyApiClient,
        IUnitsApiClient unitsApiClient,
        ICurrencyApiClient currencyApiClient,
        IPriceComponentsApiClient priceComponentsApiClient)
    {
        _propertyApiClient = propertyApiClient;
        _unitsApiClient = unitsApiClient;
        _currencyApiClient = currencyApiClient;
        _priceComponentsApiClient = priceComponentsApiClient;
    }

    public async Task<IActionResult> Index(int? propertyId, UnitStatus? status, int page = 1)
    {
        var query = new UnitQueryDto
        {
            PropertyId = propertyId,
            Status = status,
            Page = page,
            PageSize = 10
        };

        var response = await _unitsApiClient.GetPagedAsync(query);

        var properties = (await _propertyApiClient.GetAllAsync())
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        var model = new UnitListViewModel
        {
            Units = response.Items,
            PropertyId = propertyId,
            Status = status,
            Properties = properties,
            Page = page,
            TotalPages = (int)Math.Ceiling(response.TotalCount / 10.0)
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var properties = (await _propertyApiClient.GetAllAsync())
            .Where(p => p.Id == id)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        var model = new CreateUnitViewModel
        {
            Properties = properties
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUnitViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new CreateUnitDto
        {
            PropertyId = model.PropertyId,
            UnitNumber = model.UnitNumber,
            Floor = model.Floor,
            SizeSqM = model.SizeSqM,
            Status = model.Status
        };

        try
        {
            await _unitsApiClient.CreateUnitAsync(dto);
        }
        catch
        {
            ModelState.AddModelError("", "Error creating unit");
            return View(model);
        }

        return RedirectToAction("Index", new { propertyId = model.PropertyId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var unit = await _unitsApiClient.GetUnitByIdAsync(id);

        if (unit == null)
            return NotFound();

        var properties = (await _propertyApiClient.GetAllAsync())
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
            .ToList();

        var model = new EditUnitViewModel
        {
            Id = unit.Id,
            Type = unit.Type,
            UnitNumber = unit.UnitNumber,
            Area = unit.Area,
            PropertyId = unit.PropertyId,
            Floor = unit.Floor,
            Status = unit.Status,
            Properties = properties
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUnitViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new EditUnitDto
        {
            Id = model.Id,
            Type = model.Type,
            UnitNumber = model.UnitNumber,
            Area = model.Area,
            PropertyId = model.PropertyId,
            Floor = model.Floor,
            Status = model.Status
        };

        var response = await _unitsApiClient.UpdateUnitAsync(dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error updating unit");
            return View(model);
        }

        return RedirectToAction("Index", new { propertyId = model.PropertyId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        //if (!User.IsInAdminRole())
        //{
        //    return RedirectToAction("NotAuthorized", "Error");
        //}
        
        var unit = await _unitsApiClient.GetUnitByIdAsync(id);
        if (unit == null)
        {
            return RedirectToAction("BadRequest", "Error");
        }

        var model = new DeleteUnitViewModel()
        {
            Id = unit.Id,
            Type = unit.Type,
            UnitNumber = unit.UnitNumber
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _unitsApiClient.DeleteUnitById(id);

        return RedirectToAction("Index", "Properties");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var unit = await _unitsApiClient.GetUnitByIdAsync(id);

        if (unit == null)
            return NotFound();

        return View(await BuildDetailsAsync(unit));
    }

    [HttpPost]
    public async Task<IActionResult> AddPrice(
        [Bind(Prefix = nameof(UnitDetailsViewModel.PriceForm))] UnitPriceFormViewModel form)
    {
        var unit = await _unitsApiClient.GetUnitByIdAsync(form.UnitId);
        if (unit == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View("Details", await BuildDetailsAsync(unit, form));

        try
        {
            await _unitsApiClient.CreatePriceComponentAsync(new CreateUnitPriceComponentDto
            {
                UnitId = form.UnitId,
                PriceComponentId = form.PriceComponentId,
                Amount = form.Amount,
                CurrencyId = form.CurrencyId
            });
        }
        catch
        {
            ModelState.AddModelError("", "Error adding price component.");
            return View("Details", await BuildDetailsAsync(unit, form));
        }

        return RedirectToAction(nameof(Details), new { id = form.UnitId });
    }

    [HttpGet]
    public async Task<IActionResult> EditPrice(int unitId, int id)
    {
        var line = await _unitsApiClient.GetPriceComponentByIdAsync(unitId, id);
        if (line == null)
            return NotFound();

        var model = new UnitPriceFormViewModel
        {
            Id = line.Id,
            UnitId = line.UnitId,
            PriceComponentId = line.PriceComponentId,
            ComponentName = line.PriceComponentName,
            IncludeInCommission = line.IncludeInCommission,
            Amount = line.Amount,
            CurrencyId = line.CurrencyId
        };

        await model.LoadActiveCurrenciesAsync(_currencyApiClient);
        model.EnsureCurrencyOption(line.CurrencyId, CurrencyLabel(line));

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditPrice(UnitPriceFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await RestorePriceFormAsync(model);
            return View(model);
        }

        try
        {
            var response = await _unitsApiClient.UpdatePriceComponentAsync(model.UnitId, new EditUnitPriceComponentDto
            {
                Id = model.Id,
                UnitId = model.UnitId,
                Amount = model.Amount,
                CurrencyId = model.CurrencyId
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error updating price component.");
                await RestorePriceFormAsync(model);
                return View(model);
            }
        }
        catch
        {
            ModelState.AddModelError("", "Error updating price component.");
            await RestorePriceFormAsync(model);
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = model.UnitId });
    }

    [HttpPost]
    public async Task<IActionResult> DeletePrice(int unitId, int id)
    {
        try
        {
            await _unitsApiClient.DeletePriceComponentAsync(unitId, id);
        }
        catch
        {
            TempData["Error"] = "Error removing price component.";
        }

        return RedirectToAction(nameof(Details), new { id = unitId });
    }

    private async Task<UnitDetailsViewModel> BuildDetailsAsync(UnitDto unit, UnitPriceFormViewModel? form = null)
    {
        var properties = await _propertyApiClient.GetAllAsync();
        var lines = await _unitsApiClient.GetPriceComponentsAsync(unit.Id);
        var catalog = await _priceComponentsApiClient.GetAllAsync();
        var clientId = await _unitsApiClient.GetRentingClientIdAsync(unit.Id);

        form ??= new UnitPriceFormViewModel();
        form.UnitId = unit.Id;

        var assignedIds = lines.Select(line => line.PriceComponentId).ToHashSet();
        var available = catalog
            .Where(component => component.IsActive && !assignedIds.Contains(component.Id))
            .OrderBy(component => component.Name)
            .ToList();

        form.Components = available
            .Select(component => new SelectListItem
            {
                Value = component.Id.ToString(),
                Text = component.IncludeInCommission
                    ? $"{component.Name} (included)"
                    : $"{component.Name} (excluded)"
            })
            .ToList();

        await form.LoadActiveCurrenciesAsync(_currencyApiClient);

        if (form.PriceComponentId == 0 && available.Count > 0)
            form.PriceComponentId = available[0].Id;

        var totals = lines
            .GroupBy(line => line.CurrencyId)
            .Select(group =>
            {
                var sample = group.First();
                return new UnitPriceTotalViewModel
                {
                    CurrencySymbol = string.IsNullOrWhiteSpace(sample.CurrencySymbol) ? sample.CurrencyCode : sample.CurrencySymbol,
                    TenantTotal = group.Sum(line => line.Amount),
                    CommissionBase = group.Where(line => line.IncludeInCommission).Sum(line => line.Amount)
                };
            })
            .OrderBy(total => total.CurrencySymbol)
            .ToList();

        return new UnitDetailsViewModel
        {
            Id = unit.Id,
            Type = unit.Type,
            UnitNumber = unit.UnitNumber,
            Area = unit.Area,
            Floor = unit.Floor,
            Status = unit.Status.ToString(),
            PropertyId = unit.PropertyId,
            PropertyName = properties.FirstOrDefault(property => property.Id == unit.PropertyId)?.Name ?? string.Empty,
            ClientId = clientId,
            PriceLines = lines.Select(line => new UnitPriceLineViewModel
            {
                Id = line.Id,
                ComponentName = line.PriceComponentName,
                Amount = line.Amount,
                CurrencySymbol = string.IsNullOrWhiteSpace(line.CurrencySymbol) ? line.CurrencyCode : line.CurrencySymbol,
                IncludeInCommission = line.IncludeInCommission
            }).ToList(),
            Totals = totals,
            PriceForm = form,
            HasActiveCatalogComponents = catalog.Any(component => component.IsActive)
        };
    }

    private async Task RestorePriceFormAsync(UnitPriceFormViewModel model)
    {
        var line = await _unitsApiClient.GetPriceComponentByIdAsync(model.UnitId, model.Id);
        if (line != null)
        {
            model.ComponentName = line.PriceComponentName;
            model.IncludeInCommission = line.IncludeInCommission;
            model.PriceComponentId = line.PriceComponentId;
        }

        await model.LoadActiveCurrenciesAsync(_currencyApiClient);
        if (line != null)
            model.EnsureCurrencyOption(model.CurrencyId, CurrencyLabel(line));
    }

    private static string CurrencyLabel(UnitPriceComponentDto line)
    {
        return string.IsNullOrWhiteSpace(line.CurrencySymbol) ? line.CurrencyCode : line.CurrencySymbol;
    }
}

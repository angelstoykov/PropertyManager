using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Units;

public class UnitsController : Controller
{
    private readonly IPropertyApiClient _propertyApiClient;
    private readonly IUnitsApiClient _unitsApiClient;
    private readonly HttpClient _httpClient;

    public UnitsController(IPropertyApiClient propertyApiClient, IUnitsApiClient unitsApiClient, HttpClient httpClient)
    {
        _propertyApiClient = propertyApiClient;
        _unitsApiClient = unitsApiClient;
        _httpClient = httpClient;
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
        // TODO: use api
        var response = await _httpClient.PutAsJsonAsync(
            $"https://localhost:7147/api/units/{model.Id}", dto);

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
        // TODO: use api
        var response = await _httpClient.DeleteAsync($"https://localhost:7147/api/units/{id}");
        return RedirectToAction("Index", "Properties");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var unit = await _unitsApiClient.GetUnitByIdAsync(id);

        if (unit == null)
            return NotFound();

        var vm = new UnitDetailsViewModel
        {
            Id = unit.Id,
            Type = unit.Type,
            UnitNumber = unit.UnitNumber,
            Area = unit.Area,
            Floor = unit.Floor,
            Status = unit.Status.ToString(),
            PropertyId = unit.PropertyId
        };

        return View(vm);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ViewModels.Units;

public class UnitsController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IUnitsService _unitService;

    public UnitsController(IPropertyService propertyService, IUnitsService unitsService)
    {
        _propertyService = propertyService;
        _unitService = unitsService;
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

        var response = await _unitService.GetPagedAsync(query);
            //$"https://localhost:7147/api/units?propertyId={propertyId}&status={status}&page={page}&pageSize=10");

        var properties = (await _propertyService.GetAllAsync())
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        var model = new UnitListViewModel
        {
            Units = response!.Items,
            PropertyId = propertyId,
            Status = status,
            Properties = properties,
            Page = page,
            TotalPages = (int)Math.Ceiling(response.TotalCount / 10.0)
        };

        return View(model);
    }

    [HttpGet]
    public  async Task<IActionResult> Create(int id)
    {
        var properties = (await _propertyService.GetAllAsync())
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

        var response = await _httpClient.PostAsJsonAsync(
            "https://localhost:7147/api/units", dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error creating unit");
            return View(model);
        }

        return RedirectToAction("Index", new { propertyId = model.PropertyId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var unit = await _unitService.GetUnitByIdAsync(id);

        if (unit == null)
            return NotFound();

        var properties = (await _propertyService.GetAllAsync())
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

        var unit = await _unitService.GetUnitByIdAsync(id);
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

    [HttpDelete("units/{id}")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/units/{id}");
        return StatusCode((int)response.StatusCode);
    }

    //public async Task<IActionResult> Details(int id)
    //{
    //    var unit = await _unitApiClient.GetByIdAsync(id);

    //    if (unit == null)
    //        return NotFound();

    //    return View(unit);
    //}
}

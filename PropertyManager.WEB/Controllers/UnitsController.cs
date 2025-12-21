using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Enums;

public class UnitsController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly PropertyManagerDbContext _context;

    public UnitsController(HttpClient httpClient, PropertyManagerDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
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

        var response = await _httpClient.GetFromJsonAsync<PagedResult<UnitListItemDto>>(
            $"https://localhost:7147/api/units?propertyId={propertyId}&status={status}&page={page}&pageSize=10");

        var properties = _context.Properties
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        var model = new UnitIndexViewModel
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
    public IActionResult Create()
    {
        var properties = _context.Properties
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

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var unit = _context.Units
            .Include(u => u.Property)
            .FirstOrDefault(u => u.Id == id);

        if (unit == null)
            return NotFound();

        var properties = _context.Properties
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

        return RedirectToAction("Index");
    }
}

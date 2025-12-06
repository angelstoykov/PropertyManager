using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Data;
using System;

public class UnitsController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly PropertyManagerDbContext _context;

    public UnitsController(HttpClient httpClient, PropertyManagerDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
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
            "https://localhost:7001/api/units", dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error creating unit");
            return View(model);
        }

        return RedirectToAction("Index");
    }
}

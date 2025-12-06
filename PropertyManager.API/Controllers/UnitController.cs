using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.Services.Contracts;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
    {
        var id = await _unitService.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }
}

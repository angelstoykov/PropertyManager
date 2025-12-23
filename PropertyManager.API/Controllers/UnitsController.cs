using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.Services.Contracts;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly IUnitsService _unitService;

    public UnitsController(IUnitsService unitService)
    {
        _unitService = unitService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
    {
        var id = await _unitService.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] UnitQueryDto query)
    {
        var result = await _unitService.GetPagedAsync(query);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] EditUnitDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        await _unitService.EditAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _unitService.DeleteAsync(id);
        return NoContent();
    }

}

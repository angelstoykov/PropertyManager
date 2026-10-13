using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.UnitPriceComponents;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/units/{unitId}/price-components")]
    [Authorize]
    public class UnitPriceComponentsApiController : ControllerBase
    {
        private readonly IUnitPriceComponentsService _unitPriceComponentsService;

        public UnitPriceComponentsApiController(IUnitPriceComponentsService unitPriceComponentsService)
        {
            _unitPriceComponentsService = unitPriceComponentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUnit(int unitId)
        {
            var lines = await _unitPriceComponentsService.GetByUnitAsync(unitId);
            return Ok(lines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int unitId, int id)
        {
            var line = await _unitPriceComponentsService.GetByIdAsync(unitId, id);
            if (line == null)
                return NotFound();

            return Ok(line);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int unitId, [FromBody] CreateUnitPriceComponentDto dto)
        {
            if (unitId != dto.UnitId)
                return BadRequest();

            var id = await _unitPriceComponentsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { unitId, id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int unitId, int id, [FromBody] EditUnitPriceComponentDto dto)
        {
            if (unitId != dto.UnitId || id != dto.Id)
                return BadRequest();

            await _unitPriceComponentsService.EditAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int unitId, int id)
        {
            await _unitPriceComponentsService.DeleteAsync(unitId, id);
            return NoContent();
        }
    }
}

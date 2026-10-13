using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.PriceComponents;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/price-components")]
    [Authorize]
    public class PriceComponentsApiController : ControllerBase
    {
        private readonly IPriceComponentsService _priceComponentsService;

        public PriceComponentsApiController(IPriceComponentsService priceComponentsService)
        {
            _priceComponentsService = priceComponentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var components = await _priceComponentsService.GetAllAsync();
            return Ok(components);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var component = await _priceComponentsService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            return Ok(component);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePriceComponentDto dto)
        {
            var id = await _priceComponentsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditPriceComponentDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await _priceComponentsService.EditAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _priceComponentsService.DeleteAsync(id);
            return NoContent();
        }
    }
}

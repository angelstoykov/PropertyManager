using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsApiController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientsService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientsService.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            var id = await _clientsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditClientDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await _clientsService.EditAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clientsService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/units")]
        public async Task<IActionResult> GetRentedUnits(int id)
        {
            var properties = await _clientsService.GetRentedUnitsAsync(id);
            return Ok(properties);
        }

        [HttpPost("{id}/units/{unitId}")]
        public async Task<IActionResult> AddRentedUnit(int id, int unitId)
        {
            await _clientsService.AddRentedUnitAsync(id, unitId);
            return NoContent();
        }

        [HttpDelete("{id}/units/{unitId}")]
        public async Task<IActionResult> RemoveRentedUnit(int id, int unitId)
        {
            await _clientsService.RemoveRentedUnitAsync(id, unitId);
            return NoContent();
        }
    }
}

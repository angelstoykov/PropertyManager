using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Domain.Constants;
using PropertyManager.Domain.Models.Enums;

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
        public async Task<IActionResult> GetAll([FromQuery] ClientListQuery query)
        {
            var result = await _clientsService.GetAllAsync(query, CurrentAccess());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientsService.GetByIdAsync(id, CurrentAccess());
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            try
            {
                var id = await _clientsService.CreateAsync(dto, CurrentAccess());
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditClientDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            try
            {
                await _clientsService.EditAsync(dto, CurrentAccess());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            try
            {
                await _clientsService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            try
            {
                await _clientsService.DeactivateAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("available-properties")]
        public async Task<IActionResult> GetAvailableProperties()
        {
            var properties = await _clientsService.GetAvailablePropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("available-units/{propertyId}")]
        public async Task<IActionResult> GetAvailableUnits(int propertyId)
        {
            var units = await _clientsService.GetAvailableUnitsByPropertyIdAsync(propertyId);
            return Ok(units);
        }

        [HttpGet("{id}/units")]
        public async Task<IActionResult> GetRentedUnits(int id)
        {
            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            var properties = await _clientsService.GetRentedUnitsAsync(id);
            return Ok(properties);
        }

        [HttpPost("{id}/units/{unitId}")]
        public async Task<IActionResult> AddRentedUnit(int id, int unitId)
        {
            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            await _clientsService.AddRentedUnitAsync(id, unitId);
            return NoContent();
        }

        [HttpDelete("{id}/units/{unitId}")]
        public async Task<IActionResult> RemoveRentedUnit(int id, int unitId)
        {
            if (await _clientsService.GetByIdAsync(id, CurrentAccess()) == null)
                return NotFound();

            await _clientsService.RemoveRentedUnitAsync(id, unitId);
            return NoContent();
        }

        private IReadOnlyList<ClientRoleType> CurrentAccess() => ClientAccess.Resolve(User);
    }
}

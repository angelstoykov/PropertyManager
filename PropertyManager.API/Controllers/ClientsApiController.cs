using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly IUnitFinancialRecordsService _financialRecordsService;

        public ClientsApiController(
            IClientsService clientsService,
            IUnitFinancialRecordsService financialRecordsService)
        {
            _clientsService = clientsService;
            _financialRecordsService = financialRecordsService;
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

        [HttpGet("{clientId}/units/{unitId}/financial-records")]
        public async Task<IActionResult> GetFinancialRecords(int clientId, int unitId)
        {
            var records = await _financialRecordsService.GetByClientAndUnitAsync(clientId, unitId);
            return Ok(records);
        }

        [HttpGet("{clientId}/units/{unitId}/financial-records/{id}")]
        public async Task<IActionResult> GetFinancialRecord(int clientId, int unitId, int id)
        {
            var record = await _financialRecordsService.GetByIdAsync(clientId, unitId, id);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        [HttpPost("{clientId}/units/{unitId}/financial-records")]
        public async Task<IActionResult> CreateFinancialRecord(
            int clientId,
            int unitId,
            [FromBody] CreateUnitFinancialRecordDto dto)
        {
            if (unitId != dto.UnitId)
                return BadRequest();

            var id = await _financialRecordsService.CreateAsync(clientId, dto);
            return CreatedAtAction(nameof(GetFinancialRecord), new { clientId, unitId, id }, id);
        }

        [HttpPut("{clientId}/units/{unitId}/financial-records/{id}")]
        public async Task<IActionResult> EditFinancialRecord(
            int clientId,
            int unitId,
            int id,
            [FromBody] EditUnitFinancialRecordDto dto)
        {
            if (unitId != dto.UnitId || id != dto.Id)
                return BadRequest();

            await _financialRecordsService.EditAsync(clientId, dto);
            return NoContent();
        }

        [HttpDelete("{clientId}/units/{unitId}/financial-records/{id}")]
        public async Task<IActionResult> DeleteFinancialRecord(int clientId, int unitId, int id)
        {
            await _financialRecordsService.DeleteAsync(clientId, unitId, id);
            return NoContent();
        }
    }
}

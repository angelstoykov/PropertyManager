using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Domain.Constants;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/financial-records")]
    [Authorize]
    public class FinancialRecordsApiController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly IUnitFinancialRecordsService _financialRecordsService;

        public FinancialRecordsApiController(
            IClientsService clientsService,
            IUnitFinancialRecordsService financialRecordsService)
        {
            _clientsService = clientsService;
            _financialRecordsService = financialRecordsService;
        }

        [HttpGet("clients/{clientId}/units/{unitId}")]
        public async Task<IActionResult> GetFinancialRecords(int clientId, int unitId, [FromQuery] int? subtypeId)
        {
            if (await _clientsService.GetByIdAsync(clientId, CurrentAccess()) == null)
                return NotFound();

            var records = await _financialRecordsService.GetByClientAndUnitAsync(clientId, unitId, subtypeId);
            return Ok(records);
        }

        [HttpGet("clients/{clientId}/units/{unitId}/{id}")]
        public async Task<IActionResult> GetFinancialRecord(int clientId, int unitId, int id)
        {
            if (await _clientsService.GetByIdAsync(clientId, CurrentAccess()) == null)
                return NotFound();

            var record = await _financialRecordsService.GetByIdAsync(clientId, unitId, id);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        [HttpPost("clients/{clientId}/units/{unitId}")]
        public async Task<IActionResult> CreateFinancialRecord(
            int clientId,
            int unitId,
            [FromBody] CreateUnitFinancialRecordDto dto)
        {
            if (unitId != dto.UnitId)
                return BadRequest();

            if (await _clientsService.GetByIdAsync(clientId, CurrentAccess()) == null)
                return NotFound();

            try
            {
                var id = await _financialRecordsService.CreateAsync(clientId, dto);
                return CreatedAtAction(nameof(GetFinancialRecord), new { clientId, unitId, id }, id);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("clients/{clientId}/units/{unitId}/{id}")]
        public async Task<IActionResult> EditFinancialRecord(
            int clientId,
            int unitId,
            int id,
            [FromBody] EditUnitFinancialRecordDto dto)
        {
            if (unitId != dto.UnitId || id != dto.Id)
                return BadRequest();

            if (await _clientsService.GetByIdAsync(clientId, CurrentAccess()) == null)
                return NotFound();

            try
            {
                await _financialRecordsService.EditAsync(clientId, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("clients/{clientId}/units/{unitId}/{id}")]
        public async Task<IActionResult> DeleteFinancialRecord(int clientId, int unitId, int id)
        {
            if (await _clientsService.GetByIdAsync(clientId, CurrentAccess()) == null)
                return NotFound();

            await _financialRecordsService.DeleteAsync(clientId, unitId, id);
            return NoContent();
        }

        private IReadOnlyList<ClientRoleType> CurrentAccess() => ClientAccess.Resolve(User);
    }
}

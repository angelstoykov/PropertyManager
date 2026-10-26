using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Ownership;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Domain.Constants;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/ownership")]
    [Authorize]
    public class OwnershipApiController : ControllerBase
    {
        private readonly IOwnershipService _ownershipService;

        public OwnershipApiController(IOwnershipService ownershipService)
        {
            _ownershipService = ownershipService;
        }

        [HttpGet("properties/{id}")]
        public async Task<IActionResult> GetProperty(int id, [FromQuery] DateTime? date)
        {
            var denied = DenyWithoutOwnerAccess();
            if (denied != null)
                return denied;

            try
            {
                return Ok(await _ownershipService.GetPropertyOwnershipAsync(id, date));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("properties/{id}")]
        public async Task<IActionResult> SetProperty(int id, [FromBody] SetOwnershipDto dto)
        {
            var denied = DenyWithoutOwnerAccess();
            if (denied != null)
                return denied;

            try
            {
                await _ownershipService.SetPropertyOwnershipAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("units/{id}")]
        public async Task<IActionResult> GetUnit(int id, [FromQuery] DateTime? date)
        {
            var denied = DenyWithoutOwnerAccess();
            if (denied != null)
                return denied;

            try
            {
                return Ok(await _ownershipService.GetUnitOwnershipAsync(id, date));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("units/{id}")]
        public async Task<IActionResult> SetUnit(int id, [FromBody] SetOwnershipDto dto)
        {
            var denied = DenyWithoutOwnerAccess();
            if (denied != null)
                return denied;

            try
            {
                await _ownershipService.SetUnitOwnershipAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("clients/{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var denied = DenyWithoutOwnerAccess();
            if (denied != null)
                return denied;

            try
            {
                return Ok(await _ownershipService.GetClientOwnershipAsync(id));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private IActionResult? DenyWithoutOwnerAccess()
        {
            if (ClientAccess.Resolve(User).Contains(ClientRoleType.Owner))
                return null;

            return Forbid();
        }
    }
}

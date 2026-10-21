using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.FinancialRecordSubtypes;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Domain.Constants;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/financial-record-subtypes")]
    [Authorize]
    public class FinancialRecordSubtypesApiController : ControllerBase
    {
        private readonly IFinancialRecordSubtypesService _subtypesService;

        public FinancialRecordSubtypesApiController(IFinancialRecordSubtypesService subtypesService)
        {
            _subtypesService = subtypesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subtypes = await _subtypesService.GetAllAsync();
            return Ok(subtypes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subtype = await _subtypesService.GetByIdAsync(id);
            if (subtype == null)
                return NotFound();

            return Ok(subtype);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinancialRecordSubtypeDto dto)
        {
            var id = await _subtypesService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditFinancialRecordSubtypeDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await _subtypesService.EditAsync(dto);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _subtypesService.DeleteAsync(id);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.API.Controllers.Contracts;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    [Authorize]
    public class PropertiesApiController : ControllerBase, IPropertiesApiController
    {
        private readonly IPropertyService _propertyService;

        public PropertiesApiController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var properties = await _propertyService.GetAllAsync();
            return Ok(properties);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
        {
            var id = await _propertyService.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id }, id);
        }
    }
}

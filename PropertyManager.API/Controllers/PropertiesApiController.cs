using Microsoft.AspNetCore.Mvc;
using PropertyManager.API.Controllers.Contracts;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
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
    }
}

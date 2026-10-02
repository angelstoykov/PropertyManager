using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.API.Controllers.Contracts;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.Services.Contracts;

namespace PropertyManager.API.Controllers
{
    [ApiController]
    [Route("api/currencies")]
    [Authorize]
    public class CurrenciesApiController : ControllerBase, ICurrenciesApiController
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesApiController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var currencies = await _currencyService.GetAllAsync();
            return Ok(currencies);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var activeCurrencies = await _currencyService.GetAllActiveAsync();
            return Ok(activeCurrencies);
        }
    }
}

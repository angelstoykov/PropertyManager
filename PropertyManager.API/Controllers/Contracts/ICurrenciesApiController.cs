using Microsoft.AspNetCore.Mvc;

namespace PropertyManager.API.Controllers.Contracts
{
    public interface ICurrenciesApiController
    {
        Task<IActionResult> GetAllAsync();
    }
}
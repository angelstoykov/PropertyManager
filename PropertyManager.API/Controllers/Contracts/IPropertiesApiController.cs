using Microsoft.AspNetCore.Mvc;

namespace PropertyManager.API.Controllers.Contracts
{
    public interface IPropertiesApiController
    {
        Task<IActionResult> GetAllAsync();
    }
}
using Microsoft.AspNetCore.Mvc;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyApiClient _propertyApiClient;

        public PropertiesController(IPropertyApiClient propertyApiClient)
        {
            _propertyApiClient = propertyApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _propertyApiClient.GetAllAsync();
            return View(properties);
        }
    }
}

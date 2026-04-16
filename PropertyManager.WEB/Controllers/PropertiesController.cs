using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Units;

namespace PropertyManager.WEB.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IUnitsApiClient _propertyApiClient;
        private readonly HttpClient _httpClient;
        private readonly PropertyManagerDbContext _context;

        public PropertiesController(IUnitsApiClient propertyApiClient, HttpClient httpClient, PropertyManagerDbContext context)
        {
            _propertyApiClient = propertyApiClient;
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _propertyApiClient.GetAllAsync();
            return View(properties);
        }
    }
}

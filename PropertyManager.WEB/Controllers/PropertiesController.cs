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
        private readonly IPropertyApiClient _propertyApiClient;
        private readonly HttpClient _httpClient;
        private readonly PropertyManagerDbContext _context;

        public PropertiesController(IPropertyApiClient propertyApiClient, HttpClient httpClient, PropertyManagerDbContext context)
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

        public async Task<IActionResult> Index(int? propertyId, UnitStatus? status, int page = 1)
        {
            var query = new UnitQueryDto
            {
                PropertyId = propertyId,
                Status = status,
                Page = page,
                PageSize = 10
            };

            var response = await _httpClient.GetFromJsonAsync<PagedResult<UnitListItemDto>>(
                $"https://localhost:7147/api/units?propertyId={propertyId}&status={status}&page={page}&pageSize=10");

            var properties = _context.Properties
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();

            var model = new UnitIndexViewModel
            {
                Units = response!.Items,
                PropertyId = propertyId,
                Status = status,
                Properties = properties,
                Page = page,
                TotalPages = (int)Math.Ceiling(response.TotalCount / 10.0)
            };

            return View(model);
        }
    }
}

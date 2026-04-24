using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Properties;
using PropertyManager.WEB.ViewModels.Units;

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
            var properties = (await _propertyApiClient.GetAllAsync())
                .Select(p => new PropertyListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Owner = p.Owner,
                    Type = p.Type
                })
                .ToList();

            return View(properties);
        }
    }
}

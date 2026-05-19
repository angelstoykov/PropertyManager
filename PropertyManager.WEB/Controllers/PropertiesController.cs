using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Properties;

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreatePropertyViewModel
            {
                Name = string.Empty,
                Address = string.Empty,
                Owner = string.Empty,
                Type = PropertyType.Residential,
                Status = PropertyStatus.Active
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePropertyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreatePropertyDto
            {
                Name = model.Name,
                Address = model.Address,
                NumberOfUnits = model.NumberOfUnits,
                Owner = model.Owner,
                Type = PropertyType.Residential,
                Status = PropertyStatus.Active,
                Notes = model.Notes,
                Description = model.Description
            };

            try
            {
                await _propertyApiClient.CreatePropertyAsync(dto);
            }
            catch
            {
                ModelState.AddModelError("", "Error creating property");
                return View(model);
            }

            //return RedirectToAction("Index", new { propertyId = model.PropertyId });
            return RedirectToAction("Index");
        }
    }
}

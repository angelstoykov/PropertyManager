using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.PriceComponents;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.PriceComponents;

namespace PropertyManager.WEB.Controllers
{
    public class PriceComponentsController : Controller
    {
        private readonly IPriceComponentsApiClient _priceComponentsApiClient;

        public PriceComponentsController(IPriceComponentsApiClient priceComponentsApiClient)
        {
            _priceComponentsApiClient = priceComponentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await BuildIndexAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind(Prefix = nameof(PriceComponentIndexViewModel.Form))] PriceComponentFormViewModel form)
        {
            if (!ModelState.IsValid)
                return View("Index", await BuildIndexAsync(form));

            try
            {
                await _priceComponentsApiClient.CreateAsync(new CreatePriceComponentDto
                {
                    Name = form.Name,
                    IncludeInCommission = form.IncludeInCommission
                });
            }
            catch
            {
                ModelState.AddModelError("", "Error creating price component. The name may already exist.");
                return View("Index", await BuildIndexAsync(form));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var component = await _priceComponentsApiClient.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            return View(new PriceComponentFormViewModel
            {
                Id = component.Id,
                Name = component.Name,
                IncludeInCommission = component.IncludeInCommission,
                IsActive = component.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PriceComponentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _priceComponentsApiClient.UpdateAsync(new EditPriceComponentDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    IncludeInCommission = model.IncludeInCommission,
                    IsActive = model.IsActive
                });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error updating price component.");
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error updating price component. The name may already exist.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _priceComponentsApiClient.DeleteAsync(id);
            }
            catch
            {
                TempData["Error"] = "This component is assigned to a unit and cannot be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<PriceComponentIndexViewModel> BuildIndexAsync(PriceComponentFormViewModel? form = null)
        {
            var components = await _priceComponentsApiClient.GetAllAsync();
            return new PriceComponentIndexViewModel
            {
                Components = components,
                Form = form ?? new PriceComponentFormViewModel()
            };
        }
    }
}

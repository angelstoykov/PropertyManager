using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.FinancialRecordSubtypes;
using PropertyManager.Domain.Constants;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.FinancialRecordSubtypes;

namespace PropertyManager.WEB.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FinancialRecordSubtypesController : Controller
    {
        private readonly IFinancialRecordSubtypesApiClient _subtypesApiClient;

        public FinancialRecordSubtypesController(IFinancialRecordSubtypesApiClient subtypesApiClient)
        {
            _subtypesApiClient = subtypesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await BuildIndexAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind(Prefix = nameof(FinancialRecordSubtypeIndexViewModel.Form))] FinancialRecordSubtypeFormViewModel form)
        {
            if (!ModelState.IsValid)
                return View("Index", await BuildIndexAsync(form));

            try
            {
                await _subtypesApiClient.CreateAsync(new CreateFinancialRecordSubtypeDto
                {
                    Type = form.Type,
                    Name = form.Name
                });
            }
            catch
            {
                ModelState.AddModelError("", "Error creating subtype. The name may already exist for this type.");
                return View("Index", await BuildIndexAsync(form));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var subtype = await _subtypesApiClient.GetByIdAsync(id);
            if (subtype == null)
                return NotFound();

            return View(new FinancialRecordSubtypeFormViewModel
            {
                Id = subtype.Id,
                Type = subtype.Type,
                Name = subtype.Name,
                IsActive = subtype.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FinancialRecordSubtypeFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _subtypesApiClient.UpdateAsync(new EditFinancialRecordSubtypeDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    IsActive = model.IsActive
                });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error updating subtype.");
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error updating subtype. The name may already exist for this type.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _subtypesApiClient.DeleteAsync(id);
            }
            catch
            {
                TempData["Error"] = "This subtype is used by a financial record and cannot be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<FinancialRecordSubtypeIndexViewModel> BuildIndexAsync(FinancialRecordSubtypeFormViewModel? form = null)
        {
            var subtypes = await _subtypesApiClient.GetAllAsync();
            return new FinancialRecordSubtypeIndexViewModel
            {
                Subtypes = subtypes,
                Form = form ?? new FinancialRecordSubtypeFormViewModel()
            };
        }
    }
}

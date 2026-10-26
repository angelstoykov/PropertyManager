using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Ownership;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Ownership;

namespace PropertyManager.WEB.Controllers
{
    public class OwnershipController : Controller
    {
        private readonly IOwnershipApiClient _ownershipApiClient;
        private readonly IClientsApiClient _clientsApiClient;

        public OwnershipController(IOwnershipApiClient ownershipApiClient, IClientsApiClient clientsApiClient)
        {
            _ownershipApiClient = ownershipApiClient;
            _clientsApiClient = clientsApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Property(int id, DateTime? date)
        {
            try
            {
                return View(await BuildTargetPageAsync(
                    () => _ownershipApiClient.GetPropertyAsync(id, date),
                    prefillOwners: true));
            }
            catch (InvalidOperationException ex)
            {
                return OwnershipError(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetProperty(OwnershipPageViewModel model)
        {
            if (model.PropertyId is not int propertyId)
                return BadRequest();

            try
            {
                await _ownershipApiClient.SetPropertyAsync(propertyId, ToDto(model));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Property", await RebuildPostedAsync(
                    () => _ownershipApiClient.GetPropertyAsync(propertyId, null),
                    model));
            }

            return RedirectToAction(nameof(Property), new { id = propertyId });
        }

        [HttpPost]
        public async Task<IActionResult> SetUnit(OwnershipPageViewModel model)
        {
            if (model.UnitId is not int unitId)
                return BadRequest();

            try
            {
                await _ownershipApiClient.SetUnitAsync(unitId, ToDto(model));
            }
            catch (InvalidOperationException ex)
            {
                TempData["OwnershipError"] = ex.Message;
                return RedirectToAction("Details", "Units", new { id = unitId });
            }

            return RedirectToAction("Details", "Units", new { id = unitId });
        }

        private async Task<OwnershipPageViewModel> BuildTargetPageAsync(
            Func<Task<Application.DTOs.Ownership.TargetOwnershipDto>> load,
            bool prefillOwners)
        {
            var dto = await load();
            var clients = await LoadClientsAsync();
            return OwnershipDisplay.FromTarget(dto, clients, prefillOwners);
        }

        private async Task<OwnershipPageViewModel> RebuildPostedAsync(
            Func<Task<Application.DTOs.Ownership.TargetOwnershipDto>> load,
            OwnershipPageViewModel posted)
        {
            var page = await BuildTargetPageAsync(load, prefillOwners: false);
            page.EffectiveDate = posted.EffectiveDate;
            page.EndOwnership = posted.EndOwnership;
            page.Shares = posted.Shares ?? [];
            while (page.Shares.Count < 1)
                page.Shares.Add(new OwnershipShareFormRow());
            return page;
        }

        private async Task<IReadOnlyList<ClientListItemDto>> LoadClientsAsync()
        {
            var result = await _clientsApiClient.GetAllAsync(new ClientListQuery { Tab = "all" });
            return result.Clients;
        }

        private static SetOwnershipDto ToDto(OwnershipPageViewModel model)
        {
            if (!model.EndOwnership &&
                (model.Shares ?? []).Any(row => row.ClientId is > 0 && row.SharePercent is null))
            {
                throw new InvalidOperationException("Enter a share for each owner.");
            }

            var shares = model.EndOwnership
                ? []
                : (model.Shares ?? [])
                    .Where(row => row.ClientId is > 0 && row.SharePercent != null)
                    .Select(row => new OwnershipShareInputDto
                    {
                        ClientId = row.ClientId!.Value,
                        SharePercent = row.SharePercent!.Value
                    })
                    .ToList();

            if (!model.EndOwnership && shares.Count == 0)
                throw new InvalidOperationException("Add at least one owner, or end the current ownership.");

            return new SetOwnershipDto
            {
                EffectiveDate = model.EffectiveDate,
                Shares = shares
            };
        }

        private IActionResult OwnershipError(string message)
        {
            return View("Property", new OwnershipPageViewModel
            {
                Title = "Owners",
                TargetName = message
            });
        }
    }
}

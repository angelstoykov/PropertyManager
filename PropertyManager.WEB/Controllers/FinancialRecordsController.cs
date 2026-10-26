using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.FinancialRecords;

namespace PropertyManager.WEB.Controllers
{
    public class FinancialRecordsController : Controller
    {
        private readonly IFinancialRecordsApiClient _financialRecordsApiClient;
        private readonly IClientsApiClient _clientsApiClient;
        private readonly ICurrencyApiClient _currencyApiClient;
        private readonly IFinancialRecordSubtypesApiClient _subtypesApiClient;

        public FinancialRecordsController(
            IFinancialRecordsApiClient financialRecordsApiClient,
            IClientsApiClient clientsApiClient,
            ICurrencyApiClient currencyApiClient,
            IFinancialRecordSubtypesApiClient subtypesApiClient)
        {
            _financialRecordsApiClient = financialRecordsApiClient;
            _clientsApiClient = clientsApiClient;
            _currencyApiClient = currencyApiClient;
            _subtypesApiClient = subtypesApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int clientId, int unitId, int? subtypeId)
        {
            var client = await _clientsApiClient.GetByIdAsync(clientId);
            if (client == null)
                return NotFound();

            var rentedUnits = await _clientsApiClient.GetRentedUnitsAsync(clientId);
            var unit = rentedUnits.FirstOrDefault(u => u.UnitId == unitId);
            if (unit == null)
                return NotFound();

            var records = await _financialRecordsApiClient.GetFinancialRecordsAsync(clientId, unitId, subtypeId);

            var model = new UnitFinancialRecordsViewModel
            {
                ClientId = clientId,
                ClientDisplayName = GetDisplayName(client),
                UnitId = unitId,
                UnitName = unit.Name,
                UnitAddress = unit.Address,
                UnitNumber = unit.UnitNumber,
                SelectedSubtypeId = subtypeId,
                FinancialRecord = new UnitFinancialRecordFormViewModel
                {
                    ClientId = clientId,
                    UnitId = unitId,
                    Date = DateTime.Today,
                    FilterSubtypeId = subtypeId
                },
                Records = records
            };
            await model.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient, _clientsApiClient);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind(Prefix = nameof(UnitFinancialRecordsViewModel.FinancialRecord))] UnitFinancialRecordFormViewModel record)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await RebuildFinancialRecordsViewModelAsync(record));
            }

            var dto = new CreateUnitFinancialRecordDto
            {
                UnitId = record.UnitId,
                Type = record.Type,
                SubtypeId = record.SubtypeId ?? 0,
                Date = record.Date,
                Amount = record.Amount,
                CurrencyId = record.CurrencyId,
                ContractorClientId = record.ContractorClientId,
                Comment = record.Comment
            };

            try
            {
                await _financialRecordsApiClient.CreateFinancialRecordAsync(record.ClientId, dto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", await RebuildFinancialRecordsViewModelAsync(record));
            }
            catch
            {
                ModelState.AddModelError("", "Error creating financial record.");
                return View("Index", await RebuildFinancialRecordsViewModelAsync(record));
            }

            return RedirectToAction(nameof(Index), new
            {
                clientId = record.ClientId,
                unitId = record.UnitId,
                subtypeId = record.FilterSubtypeId
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int clientId, int unitId, int id, int? subtypeId)
        {
            var record = await _financialRecordsApiClient.GetFinancialRecordByIdAsync(clientId, unitId, id);
            if (record == null)
                return NotFound();

            var model = new UnitFinancialRecordFormViewModel
            {
                Id = record.Id,
                ClientId = clientId,
                UnitId = unitId,
                Type = record.Type,
                SubtypeId = record.SubtypeId,
                Date = record.Date,
                Amount = record.Amount,
                CurrencyId = record.CurrencyId,
                ContractorClientId = record.ContractorClientId,
                Comment = record.Comment,
                FilterSubtypeId = subtypeId
            };

            await LoadFinancialRecordLookupsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UnitFinancialRecordFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadFinancialRecordLookupsAsync(model);
                return View(model);
            }

            var dto = new EditUnitFinancialRecordDto
            {
                Id = model.Id,
                UnitId = model.UnitId,
                Type = model.Type,
                SubtypeId = model.SubtypeId ?? 0,
                Date = model.Date,
                Amount = model.Amount,
                CurrencyId = model.CurrencyId,
                ContractorClientId = model.ContractorClientId,
                Comment = model.Comment
            };

            try
            {
                var response = await _financialRecordsApiClient.UpdateFinancialRecordAsync(model.ClientId, model.UnitId, dto);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error updating financial record.");
                    await LoadFinancialRecordLookupsAsync(model);
                    return View(model);
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadFinancialRecordLookupsAsync(model);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Error updating financial record.");
                await LoadFinancialRecordLookupsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new
            {
                clientId = model.ClientId,
                unitId = model.UnitId,
                subtypeId = model.FilterSubtypeId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int clientId, int unitId, int id, int? subtypeId)
        {
            try
            {
                await _financialRecordsApiClient.DeleteFinancialRecordAsync(clientId, unitId, id);
            }
            catch
            {
                return RedirectToAction(nameof(Index), new { clientId, unitId, subtypeId });
            }

            return RedirectToAction(nameof(Index), new { clientId, unitId, subtypeId });
        }

        private async Task LoadFinancialRecordLookupsAsync(UnitFinancialRecordFormViewModel model)
        {
            await model.LoadActiveCurrenciesAsync(_currencyApiClient);
            await model.LoadSubtypesAsync(_subtypesApiClient);
            await model.LoadContractorsAsync(_clientsApiClient);
        }

        private async Task<UnitFinancialRecordsViewModel> RebuildFinancialRecordsViewModelAsync(UnitFinancialRecordFormViewModel model)
        {
            var client = await _clientsApiClient.GetByIdAsync(model.ClientId);
            var rentedUnits = await _clientsApiClient.GetRentedUnitsAsync(model.ClientId);
            var unit = rentedUnits.First(u => u.UnitId == model.UnitId);
            var records = await _financialRecordsApiClient.GetFinancialRecordsAsync(
                model.ClientId, model.UnitId, model.FilterSubtypeId);

            var vm = new UnitFinancialRecordsViewModel
            {
                ClientId = model.ClientId,
                ClientDisplayName = client == null ? string.Empty : GetDisplayName(client),
                UnitId = model.UnitId,
                UnitName = unit.Name,
                UnitAddress = unit.Address,
                SelectedSubtypeId = model.FilterSubtypeId,
                FinancialRecord = model,
                Records = records
            };
            await vm.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient, _clientsApiClient);
            return vm;
        }

        private static string GetDisplayName(ClientDto client) =>
            client.ClientType == ClientType.Company
                ? client.CompanyName ?? string.Empty
                : $"{client.FirstName} {client.LastName}".Trim();
    }
}

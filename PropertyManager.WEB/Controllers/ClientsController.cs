using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Clients;

namespace PropertyManager.WEB.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientsApiClient _clientsApiClient;
        private readonly ICurrencyApiClient _currencyApiClient;
        private readonly IFinancialRecordSubtypesApiClient _subtypesApiClient;

        public ClientsController(IClientsApiClient clientsApiClient,
                                 ICurrencyApiClient currencyApiClient,
                                 IFinancialRecordSubtypesApiClient subtypesApiClient)
        {
            _clientsApiClient = clientsApiClient;
            _currencyApiClient = currencyApiClient;
            _subtypesApiClient = subtypesApiClient;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var clients = (await _clientsApiClient.GetAllAsync(search))
                .Select(c => new ClientListItemViewModel
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    DisplayName = c.DisplayName,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToList();

            return View(new ClientsIndexViewModel
            {
                Search = search,
                Clients = clients
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClientFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = MapToCreateDto(model);

            try
            {
                await _clientsApiClient.CreateAsync(dto);
            }
            catch
            {
                ModelState.AddModelError("", "Error creating client.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return View(MapToFormViewModel(client));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = MapToEditDto(model);

            try
            {
                var response = await _clientsApiClient.UpdateAsync(dto);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error updating client.");
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error updating client.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            var model = new DeleteClientViewModel
            {
                Id = client.Id,
                ClientType = client.ClientType,
                DisplayName = GetDisplayName(client)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _clientsApiClient.DeleteAsync(id);
            }
            catch
            {
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RentedUnits(int id)
        {
            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return View(await BuildRentedUnitsViewModelAsync(client));
        }

        [HttpPost]
        public async Task<IActionResult> AddRentedUnit(int clientId, int unitId)
        {
            await _clientsApiClient.AddRentedUnitAsync(clientId, unitId);
            return RedirectToAction(nameof(RentedUnits), new { id = clientId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableUnits(int propertyId)
        {
            var units = await _clientsApiClient.GetAvailableUnitsByPropertyIdAsync(propertyId);
            return Json(units);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRentedUnit(int clientId, int unitId)
        {
            await _clientsApiClient.RemoveRentedUnitAsync(clientId, unitId);
            return RedirectToAction(nameof(RentedUnits), new { id = clientId });
        }

        [HttpGet]
        public async Task<IActionResult> UnitFinancialRecords(int clientId, int unitId, int? subtypeId)
        {
            var client = await _clientsApiClient.GetByIdAsync(clientId);
            if (client == null)
                return NotFound();

            var rentedUnits = await _clientsApiClient.GetRentedUnitsAsync(clientId);
            var unit = rentedUnits.FirstOrDefault(u => u.UnitId == unitId);
            if (unit == null)
                return NotFound();

            var records = await _clientsApiClient.GetFinancialRecordsAsync(clientId, unitId, subtypeId);

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
            await model.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFinancialRecord(
            [Bind(Prefix = nameof(UnitFinancialRecordsViewModel.FinancialRecord))] UnitFinancialRecordFormViewModel record)
        {
            if (!ModelState.IsValid)
            {
                return View("UnitFinancialRecords", await RebuildFinancialRecordsViewModelAsync(record));
            }
            var dto = new CreateUnitFinancialRecordDto
            {
                UnitId = record.UnitId,
                Type = record.Type,
                SubtypeId = record.SubtypeId ?? 0,
                Date = record.Date,
                Amount = record.Amount,
                CurrencyId = record.CurrencyId,
                Comment = record.Comment
            };

            try
            {
                await _clientsApiClient.CreateFinancialRecordAsync(record.ClientId, dto);
            }
            catch
            {
                ModelState.AddModelError("", "Error creating financial record.");
                return View("UnitFinancialRecords", await RebuildFinancialRecordsViewModelAsync(record));
            }

            return RedirectToAction(nameof(UnitFinancialRecords), new
            {
                clientId = record.ClientId,
                unitId = record.UnitId,
                subtypeId = record.FilterSubtypeId
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditFinancialRecord(int clientId, int unitId, int id, int? subtypeId)
        {
            var record = await _clientsApiClient.GetFinancialRecordByIdAsync(clientId, unitId, id);
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
                Comment = record.Comment,
                FilterSubtypeId = subtypeId
            };

            await model.LoadActiveCurrenciesAsync(_currencyApiClient);
            await model.LoadSubtypesAsync(_subtypesApiClient);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditFinancialRecord(UnitFinancialRecordFormViewModel model)
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
                Comment = model.Comment
            };

            try
            {
                var response = await _clientsApiClient.UpdateFinancialRecordAsync(model.ClientId, model.UnitId, dto);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error updating financial record.");
                    await LoadFinancialRecordLookupsAsync(model);
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error updating financial record.");
                await LoadFinancialRecordLookupsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(UnitFinancialRecords), new
            {
                clientId = model.ClientId,
                unitId = model.UnitId,
                subtypeId = model.FilterSubtypeId
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFinancialRecord(int clientId, int unitId, int id, int? subtypeId)
        {
            try
            {
                await _clientsApiClient.DeleteFinancialRecordAsync(clientId, unitId, id);
            }
            catch
            {
                return RedirectToAction(nameof(UnitFinancialRecords), new { clientId, unitId, subtypeId });
            }

            return RedirectToAction(nameof(UnitFinancialRecords), new { clientId, unitId, subtypeId });
        }

        private async Task LoadFinancialRecordLookupsAsync(UnitFinancialRecordFormViewModel model)
        {
            await model.LoadActiveCurrenciesAsync(_currencyApiClient);
            await model.LoadSubtypesAsync(_subtypesApiClient);
        }

        private async Task<UnitFinancialRecordsViewModel> RebuildFinancialRecordsViewModelAsync(UnitFinancialRecordFormViewModel model)
        {
            var client = await _clientsApiClient.GetByIdAsync(model.ClientId);
            var rentedUnits = await _clientsApiClient.GetRentedUnitsAsync(model.ClientId);
            var unit = rentedUnits.First(u => u.UnitId == model.UnitId);
            var records = await _clientsApiClient.GetFinancialRecordsAsync(
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
            await vm.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient);
            return vm;
        }

        private static CreateClientDto MapToCreateDto(ClientFormViewModel model) => new()
        {
            ClientType = model.ClientType,
            Email = model.Email,
            Phone = model.Phone,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PersonalId = model.PersonalId,
            CompanyName = model.CompanyName,
            CompanyNumber = model.CompanyNumber,
            VatNumber = model.VatNumber,
            LegalRepresentative = model.LegalRepresentative
        };

        private static EditClientDto MapToEditDto(ClientFormViewModel model) => new()
        {
            Id = model.Id,
            ClientType = model.ClientType,
            Email = model.Email,
            Phone = model.Phone,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PersonalId = model.PersonalId,
            CompanyName = model.CompanyName,
            CompanyNumber = model.CompanyNumber,
            VatNumber = model.VatNumber,
            LegalRepresentative = model.LegalRepresentative
        };

        private static ClientFormViewModel MapToFormViewModel(ClientDto client) => new()
        {
            Id = client.Id,
            ClientType = client.ClientType,
            Email = client.Email,
            Phone = client.Phone,
            FirstName = client.FirstName,
            LastName = client.LastName,
            PersonalId = client.PersonalId,
            CompanyName = client.CompanyName,
            CompanyNumber = client.CompanyNumber,
            VatNumber = client.VatNumber,
            LegalRepresentative = client.LegalRepresentative
        };

        private static string GetDisplayName(ClientDto client) =>
            client.ClientType == ClientType.LegalEntity
                ? client.CompanyName ?? string.Empty
                : $"{client.FirstName} {client.LastName}".Trim();

        private async Task<ClientRentedUnitsViewModel> BuildRentedUnitsViewModelAsync(ClientDto client)
        {
            var rentedUnits = await _clientsApiClient.GetRentedUnitsAsync(client.Id);
            var availableProperties = await _clientsApiClient.GetAvailablePropertiesAsync();

            return new ClientRentedUnitsViewModel
            {
                ClientId = client.Id,
                ClientType = client.ClientType,
                ClientDisplayName = GetDisplayName(client),
                RentedUnits = rentedUnits,
                AvailableProperties = availableProperties
            };
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Ownership;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Domain.Constants;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Clients;
using PropertyManager.WEB.ViewModels.Ownership;

namespace PropertyManager.WEB.Controllers
{
    public class ClientsController : Controller
    {
        private const string TabCookieName = "pm.clients.tab";

        private readonly IClientsApiClient _clientsApiClient;
        private readonly ICurrencyApiClient _currencyApiClient;
        private readonly IFinancialRecordSubtypesApiClient _subtypesApiClient;
        private readonly IPropertyApiClient _propertyApiClient;
        private readonly IOwnershipApiClient _ownershipApiClient;

        public ClientsController(IClientsApiClient clientsApiClient,
                                 ICurrencyApiClient currencyApiClient,
                                 IFinancialRecordSubtypesApiClient subtypesApiClient,
                                 IPropertyApiClient propertyApiClient,
                                 IOwnershipApiClient ownershipApiClient)
        {
            _clientsApiClient = clientsApiClient;
            _currencyApiClient = currencyApiClient;
            _subtypesApiClient = subtypesApiClient;
            _propertyApiClient = propertyApiClient;
            _ownershipApiClient = ownershipApiClient;
        }

        public async Task<IActionResult> Index(
            string? tab,
            string? search,
            bool searchAll,
            string? status,
            ClientType? clientType,
            int? buildingId)
        {
            var allowed = CurrentAccess();
            var permitted = PermittedTabs(allowed);
            if (permitted.Count == 0)
                return View(new ClientsIndexViewModel { AccessDenied = true });

            var normalized = NormalizeTab(tab);
            status = NormalizeStatus(status);
            if (string.IsNullOrEmpty(normalized) || !permitted.Contains(normalized))
            {
                var remembered = ReadRememberedTab();
                var target = remembered != null && permitted.Contains(remembered)
                    ? remembered
                    : permitted[0];
                return RedirectToAction(nameof(Index), new
                {
                    tab = target,
                    search,
                    searchAll = searchAll ? true : (bool?)null,
                    status,
                    clientType,
                    buildingId
                });
            }

            RememberTab(normalized);

            var result = await _clientsApiClient.GetAllAsync(new ClientListQuery
            {
                Tab = normalized,
                Search = search,
                SearchAll = searchAll,
                Status = status,
                ClientType = clientType,
                BuildingId = buildingId
            });
            var properties = await _propertyApiClient.GetAllAsync();

            return View(new ClientsIndexViewModel
            {
                Tab = normalized,
                Search = search,
                SearchAll = searchAll,
                Status = status,
                ClientType = clientType,
                BuildingId = buildingId,
                CreateRole = CreateRoleFor(normalized),
                CanViewOwnership = allowed.Contains(ClientRoleType.Owner),
                Columns = ColumnsFor(normalized),
                Tabs = permitted.Select(key => new ClientTabViewModel
                {
                    Key = key,
                    Label = TabLabel(key),
                    Count = CountFor(key, result)
                }).ToList(),
                Buildings = properties
                    .OrderBy(property => property.Name)
                    .Select(property => new SelectListItem(property.Name, property.Id.ToString()))
                    .ToList(),
                Clients = result.Clients.Select(c => new ClientListItemViewModel
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    DisplayName = c.DisplayName,
                    Email = c.Email,
                    Phone = c.Phone,
                    IsActive = c.IsActive,
                    HasLinkedRecords = c.HasLinkedRecords,
                    RentedUnits = string.Join(", ", c.RentedUnitLabels),
                    Ownership = string.Join(", ", c.OwnershipLabels),
                    Roles = c.Roles.Select(ClientRolePresentation.ToBadge).ToList()
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Ownership(int id)
        {
            if (!CurrentAccess().Contains(ClientRoleType.Owner))
                return Forbid();

            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            var ownership = await _ownershipApiClient.GetClientAsync(id);
            var today = DateTime.Today;
            ClientOwnershipRowViewModel Map(ClientOwnershipItemDto item) => new()
            {
                TargetName = item.TargetName,
                Kind = item.Kind,
                Share = OwnershipDisplay.Share(item.SharePercent),
                Period = OwnershipDisplay.Period(item.StartDate, item.EndDate),
                PropertyId = item.PropertyId,
                UnitId = item.UnitId
            };

            return View(new ClientOwnershipPageViewModel
            {
                ClientId = ownership.ClientId,
                ClientName = ownership.ClientName,
                Current = ownership.Items
                    .Where(item => item.StartDate <= today && (item.EndDate == null || today < item.EndDate))
                    .Select(Map)
                    .ToList(),
                History = ownership.Items.Select(Map).ToList()
            });
        }

        [HttpGet]
        public IActionResult Create(string? role)
        {
            var model = new ClientFormViewModel();
            var allowed = CurrentAccess();
            if (Enum.TryParse<ClientRoleType>(role, ignoreCase: true, out var parsed) && allowed.Contains(parsed))
                model.SelectedRoles = [parsed];
            else if (allowed.Count == 1)
                model.SelectedRoles = [allowed[0]];

            ApplyRoleOptions(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ApplyRoleOptions(model, null);
                return View(model);
            }

            var dto = MapToCreateDto(model);

            try
            {
                await _clientsApiClient.CreateAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ApplyRoleOptions(model, null);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Error creating client.");
                ApplyRoleOptions(model, null);
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

            var model = MapToFormViewModel(client);
            var allowed = CurrentAccess();
            model.SelectedRoles = client.Roles
                .Where(role => role.IsActive && allowed.Contains(role.Role))
                .Select(role => role.Role)
                .ToList();
            ApplyRoleOptions(model, client.Roles);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClientFormViewModel model)
        {
            var client = await _clientsApiClient.GetByIdAsync(model.Id);
            if (client == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ApplyRoleOptions(model, client.Roles);
                return View(model);
            }

            var dto = MapToEditDto(model);

            try
            {
                await _clientsApiClient.UpdateAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ApplyRoleOptions(model, client.Roles);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Error updating client.");
                ApplyRoleOptions(model, client.Roles);
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

            if (client.HasLinkedRecords)
                return RedirectToAction(nameof(Deactivate), new { id });

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
            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            if (client.HasLinkedRecords)
                return RedirectToAction(nameof(Deactivate), new { id });

            try
            {
                await _clientsApiClient.DeleteAsync(id);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction(nameof(Deactivate), new { id });
            }
            catch
            {
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Deactivate(int id)
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
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            try
            {
                await _clientsApiClient.DeactivateAsync(id);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction(nameof(Deactivate), new { id });
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
            await model.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient, _clientsApiClient);
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
                ContractorClientId = record.ContractorClientId,
                Comment = record.Comment
            };

            try
            {
                await _clientsApiClient.CreateFinancialRecordAsync(record.ClientId, dto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UnitFinancialRecords", await RebuildFinancialRecordsViewModelAsync(record));
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
                ContractorClientId = record.ContractorClientId,
                Comment = record.Comment,
                FilterSubtypeId = subtypeId
            };

            await LoadFinancialRecordLookupsAsync(model);

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
                ContractorClientId = model.ContractorClientId,
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
            await model.LoadContractorsAsync(_clientsApiClient);
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
            await vm.LoadLookupsAsync(_currencyApiClient, _subtypesApiClient, _clientsApiClient);
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
            DateOfBirth = model.DateOfBirth,
            CompanyName = model.CompanyName,
            CompanyNumber = model.CompanyNumber,
            VatNumber = model.VatNumber,
            LegalRepresentative = model.LegalRepresentative,
            Address = model.Address,
            BankAccount = model.BankAccount,
            Notes = model.Notes,
            Roles = model.SelectedRoles
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
            DateOfBirth = model.DateOfBirth,
            CompanyName = model.CompanyName,
            CompanyNumber = model.CompanyNumber,
            VatNumber = model.VatNumber,
            LegalRepresentative = model.LegalRepresentative,
            Address = model.Address,
            BankAccount = model.BankAccount,
            Notes = model.Notes,
            IsActive = model.IsActive,
            Roles = model.SelectedRoles
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
            DateOfBirth = client.DateOfBirth,
            CompanyName = client.CompanyName,
            CompanyNumber = client.CompanyNumber,
            VatNumber = client.VatNumber,
            LegalRepresentative = client.LegalRepresentative,
            Address = client.Address,
            BankAccount = client.BankAccount,
            Notes = client.Notes,
            IsActive = client.IsActive
        };

        private static string GetDisplayName(ClientDto client) =>
            client.ClientType == ClientType.Company
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
                Roles = client.Roles.Select(ClientRolePresentation.ToBadge).ToList(),
                RentedUnits = rentedUnits,
                AvailableProperties = availableProperties
            };
        }

        private IReadOnlyList<ClientRoleType> CurrentAccess() => ClientAccess.Resolve(User);

        private void ApplyRoleOptions(ClientFormViewModel model, IReadOnlyList<ClientRoleDto>? roles)
        {
            var allowed = CurrentAccess();
            model.AssignableRoles = allowed.ToList();
            model.RoleDetails = allowed.Select(role =>
            {
                var existing = roles?.FirstOrDefault(item => item.Role == role);
                return new ClientRoleOptionViewModel
                {
                    Role = role,
                    StartDate = existing?.StartDate,
                    HasRole = existing != null,
                    IsActive = existing?.IsActive ?? false
                };
            }).ToList();
        }

        private void RememberTab(string tab)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            Response.Cookies.Append(TabCookieName, $"{userId}|{tab}", new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Path = "/"
            });
        }

        private string? ReadRememberedTab()
        {
            if (!Request.Cookies.TryGetValue(TabCookieName, out var value))
                return null;

            var separator = value.IndexOf('|');
            if (separator < 0)
                return null;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!string.Equals(value[..separator], userId, StringComparison.Ordinal))
                return null;

            return NormalizeTab(value[(separator + 1)..]);
        }

        private static IReadOnlyList<string> PermittedTabs(IReadOnlyList<ClientRoleType> allowed)
        {
            if (allowed.Count == 0)
                return [];

            var tabs = new List<string> { "all" };
            if (allowed.Contains(ClientRoleType.Owner))
                tabs.Add("owners");
            if (allowed.Contains(ClientRoleType.Tenant))
                tabs.Add("tenants");
            if (allowed.Contains(ClientRoleType.Contractor))
                tabs.Add("contractors");
            return tabs;
        }

        private static string NormalizeTab(string? tab) => tab?.Trim().ToLowerInvariant() switch
        {
            "all" => "all",
            "owners" or "owner" => "owners",
            "tenants" or "tenant" => "tenants",
            "contractors" or "contractor" => "contractors",
            _ => string.Empty
        };

        private static string? NormalizeStatus(string? status) => status?.Trim().ToLowerInvariant() switch
        {
            "active" => "active",
            "inactive" => "inactive",
            _ => null
        };

        private static ClientRoleType? CreateRoleFor(string tab) => tab switch
        {
            "owners" => ClientRoleType.Owner,
            "tenants" => ClientRoleType.Tenant,
            "contractors" => ClientRoleType.Contractor,
            _ => null
        };

        private static IReadOnlyList<string> ColumnsFor(string tab) => tab switch
        {
            "owners" => ["Name", "Roles", "Ownership", "Email", "Phone", "Status"],
            "tenants" => ["Name", "Roles", "RentedUnits", "Phone", "Status"],
            "contractors" => ["Name", "Type", "Roles", "Phone", "Email", "Status"],
            _ => ["Name", "Type", "Roles", "Email", "Phone", "Status"]
        };

        private static string TabLabel(string tab) => tab switch
        {
            "owners" => "Owners",
            "tenants" => "Tenants",
            "contractors" => "Contractors",
            _ => "All"
        };

        private static int CountFor(string tab, ClientListResultDto result) => tab switch
        {
            "owners" => result.OwnerCount,
            "tenants" => result.TenantCount,
            "contractors" => result.ContractorCount,
            _ => result.AllCount
        };
    }
}

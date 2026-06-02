using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.ViewModels.Clients;

namespace PropertyManager.WEB.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientsApiClient _clientsApiClient;
        private readonly IPropertyApiClient _propertyApiClient;

        public ClientsController(IClientsApiClient clientsApiClient, IPropertyApiClient propertyApiClient)
        {
            _clientsApiClient = clientsApiClient;
            _propertyApiClient = propertyApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var clients = (await _clientsApiClient.GetAllAsync())
                .Select(c => new ClientListItemViewModel
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    DisplayName = c.DisplayName,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToList();

            return View(clients);
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
        public async Task<IActionResult> RentedProperties(int id)
        {
            var client = await _clientsApiClient.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return View(await BuildRentedPropertiesViewModelAsync(client));
        }

        [HttpPost]
        public async Task<IActionResult> AddRentedProperty(int clientId, int propertyId)
        {
            await _clientsApiClient.AddRentedPropertyAsync(clientId, propertyId);
            return RedirectToAction(nameof(RentedProperties), new { id = clientId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRentedProperty(int clientId, int propertyId)
        {
            await _clientsApiClient.RemoveRentedPropertyAsync(clientId, propertyId);
            return RedirectToAction(nameof(RentedProperties), new { id = clientId });
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

        private async Task<ClientRentedPropertiesViewModel> BuildRentedPropertiesViewModelAsync(ClientDto client)
        {
            var rentedProperties = await _clientsApiClient.GetRentedPropertiesAsync(client.Id);
            var allProperties = (await _propertyApiClient.GetAllAsync()).ToList();
            var rentedPropertyIds = rentedProperties.Select(rp => rp.PropertyId).ToHashSet();
            var availableProperties = allProperties
                .Where(p => !rentedPropertyIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .ToList();

            return new ClientRentedPropertiesViewModel
            {
                ClientId = client.Id,
                ClientType = client.ClientType,
                ClientDisplayName = GetDisplayName(client),
                RentedProperties = rentedProperties,
                AvailableProperties = availableProperties
            };
        }
    }
}

using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IClientsApiClient
    {
        Task<IReadOnlyList<ClientListItemDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateClientDto dto);
        Task<HttpResponseMessage> UpdateAsync(EditClientDto dto);
        Task DeleteAsync(int id);
        Task<IReadOnlyList<ClientRentedUnitDto>> GetRentedUnitsAsync(int clientId);
        Task<IReadOnlyList<PropertyListItemDto>> GetAvailablePropertiesAsync();
        Task<IReadOnlyList<UnitListItemDto>> GetAvailableUnitsByPropertyIdAsync(int propertyId);
        Task AddRentedUnitAsync(int clientId, int unitId);
        Task RemoveRentedUnitAsync(int clientId, int unitId);
    }
}

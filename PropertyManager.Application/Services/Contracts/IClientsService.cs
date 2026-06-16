using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IClientsService
    {
        Task<IReadOnlyList<ClientListItemDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateClientDto dto);
        Task EditAsync(EditClientDto dto);
        Task DeleteAsync(int id);
        Task<IReadOnlyList<ClientRentedUnitDto>> GetRentedUnitsAsync(int clientId);
        Task<IReadOnlyList<PropertyListItemDto>> GetAvailablePropertiesAsync();
        Task<IReadOnlyList<UnitListItemDto>> GetAvailableUnitsByPropertyIdAsync(int propertyId);
        Task AddRentedUnitAsync(int clientId, int unitId);
        Task RemoveRentedUnitAsync(int clientId, int unitId);
    }
}

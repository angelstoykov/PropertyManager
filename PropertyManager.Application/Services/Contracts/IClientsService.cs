using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IClientsService
    {
        Task<ClientListResultDto> GetAllAsync(ClientListQuery query, IReadOnlyList<ClientRoleType> allowedRoles);
        Task<ClientDto?> GetByIdAsync(int id, IReadOnlyList<ClientRoleType> allowedRoles);
        Task<int> CreateAsync(CreateClientDto dto, IReadOnlyList<ClientRoleType> allowedRoles);
        Task EditAsync(EditClientDto dto, IReadOnlyList<ClientRoleType> allowedRoles);
        Task DeleteAsync(int id);
        Task DeactivateAsync(int id);
        Task EnsureRoleAsync(int clientId, ClientRoleType role, DateTime? startDate = null);
        Task<IReadOnlyList<ClientRentedUnitDto>> GetRentedUnitsAsync(int clientId);
        Task<IReadOnlyList<PropertyListItemDto>> GetAvailablePropertiesAsync();
        Task<IReadOnlyList<UnitListItemDto>> GetAvailableUnitsByPropertyIdAsync(int propertyId);
        Task AddRentedUnitAsync(int clientId, int unitId);
        Task RemoveRentedUnitAsync(int clientId, int unitId);
    }
}

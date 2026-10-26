using PropertyManager.Application.DTOs.Ownership;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IOwnershipService
    {
        Task<TargetOwnershipDto> GetPropertyOwnershipAsync(int propertyId, DateTime? onDate);
        Task<TargetOwnershipDto> GetUnitOwnershipAsync(int unitId, DateTime? onDate);
        Task SetPropertyOwnershipAsync(int propertyId, SetOwnershipDto dto);
        Task SetUnitOwnershipAsync(int unitId, SetOwnershipDto dto);
        Task<ClientOwnershipDto> GetClientOwnershipAsync(int clientId);
    }
}

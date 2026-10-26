using PropertyManager.Application.DTOs.Ownership;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IOwnershipApiClient
    {
        Task<TargetOwnershipDto> GetPropertyAsync(int propertyId, DateTime? date);
        Task SetPropertyAsync(int propertyId, SetOwnershipDto dto);
        Task<TargetOwnershipDto> GetUnitAsync(int unitId, DateTime? date);
        Task SetUnitAsync(int unitId, SetOwnershipDto dto);
        Task<ClientOwnershipDto> GetClientAsync(int clientId);
    }
}

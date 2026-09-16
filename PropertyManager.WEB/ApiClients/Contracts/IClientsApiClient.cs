using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.DTOs.UnitFinancialRecords;

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
        Task<IReadOnlyList<UnitFinancialRecordDto>> GetFinancialRecordsAsync(int clientId, int unitId);
        Task<UnitFinancialRecordDto?> GetFinancialRecordByIdAsync(int clientId, int unitId, int id);
        Task CreateFinancialRecordAsync(int clientId, CreateUnitFinancialRecordDto dto);
        Task<HttpResponseMessage> UpdateFinancialRecordAsync(int clientId, int unitId, EditUnitFinancialRecordDto dto);
        Task DeleteFinancialRecordAsync(int clientId, int unitId, int id);
    }
}

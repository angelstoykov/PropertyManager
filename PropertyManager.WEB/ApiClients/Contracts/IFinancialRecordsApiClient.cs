using PropertyManager.Application.DTOs.UnitFinancialRecords;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IFinancialRecordsApiClient
    {
        Task<IReadOnlyList<UnitFinancialRecordDto>> GetFinancialRecordsAsync(int clientId, int unitId, int? subtypeId = null);
        Task<UnitFinancialRecordDto?> GetFinancialRecordByIdAsync(int clientId, int unitId, int id);
        Task CreateFinancialRecordAsync(int clientId, CreateUnitFinancialRecordDto dto);
        Task<HttpResponseMessage> UpdateFinancialRecordAsync(int clientId, int unitId, EditUnitFinancialRecordDto dto);
        Task DeleteFinancialRecordAsync(int clientId, int unitId, int id);
    }
}

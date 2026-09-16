using PropertyManager.Application.DTOs.UnitFinancialRecords;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IUnitFinancialRecordsService
    {
        Task<IReadOnlyList<UnitFinancialRecordDto>> GetByClientAndUnitAsync(int clientId, int unitId);
        Task<UnitFinancialRecordDto?> GetByIdAsync(int clientId, int unitId, int id);
        Task<int> CreateAsync(int clientId, CreateUnitFinancialRecordDto dto);
        Task EditAsync(int clientId, EditUnitFinancialRecordDto dto);
        Task DeleteAsync(int clientId, int unitId, int id);
    }
}

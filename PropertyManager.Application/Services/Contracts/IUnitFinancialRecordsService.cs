using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IUnitFinancialRecordsService
    {
        Task<IReadOnlyList<UnitFinancialRecordDto>> GetByClientAndUnitAsync(int clientId, int unitId, int? subtypeId = null);
        Task<UnitFinancialRecordDto?> GetByIdAsync(int clientId, int unitId, int id);
        Task<int> CreateAsync(int clientId, CreateUnitFinancialRecordDto dto);
        Task EditAsync(int clientId, EditUnitFinancialRecordDto dto);
        Task DeleteAsync(int clientId, int unitId, int id);
        Task<IReadOnlyList<BuildingFinancialTotalDto>> GetBuildingTotalsAsync(
            DateTime startDate,
            DateTime endDate,
            IReadOnlyCollection<int>? propertyIds,
            FinancialRecordType? type);
    }
}

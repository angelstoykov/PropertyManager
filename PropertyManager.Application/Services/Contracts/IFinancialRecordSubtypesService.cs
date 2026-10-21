using PropertyManager.Application.DTOs.FinancialRecordSubtypes;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IFinancialRecordSubtypesService
    {
        Task<IReadOnlyList<FinancialRecordSubtypeDto>> GetAllAsync();
        Task<FinancialRecordSubtypeDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateFinancialRecordSubtypeDto dto);
        Task EditAsync(EditFinancialRecordSubtypeDto dto);
        Task DeleteAsync(int id);
    }
}

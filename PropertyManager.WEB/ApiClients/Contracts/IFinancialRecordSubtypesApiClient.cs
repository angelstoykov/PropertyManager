using PropertyManager.Application.DTOs.FinancialRecordSubtypes;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IFinancialRecordSubtypesApiClient
    {
        Task<IReadOnlyList<FinancialRecordSubtypeDto>> GetAllAsync();
        Task<FinancialRecordSubtypeDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateFinancialRecordSubtypeDto dto);
        Task<HttpResponseMessage> UpdateAsync(EditFinancialRecordSubtypeDto dto);
        Task DeleteAsync(int id);
    }
}

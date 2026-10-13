using PropertyManager.Application.DTOs.PriceComponents;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IPriceComponentsService
    {
        Task<IReadOnlyList<PriceComponentDto>> GetAllAsync();
        Task<PriceComponentDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreatePriceComponentDto dto);
        Task EditAsync(EditPriceComponentDto dto);
        Task DeleteAsync(int id);
    }
}

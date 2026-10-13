using PropertyManager.Application.DTOs.UnitPriceComponents;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IUnitPriceComponentsService
    {
        Task<IReadOnlyList<UnitPriceComponentDto>> GetByUnitAsync(int unitId);
        Task<UnitPriceComponentDto?> GetByIdAsync(int unitId, int id);
        Task<int> CreateAsync(CreateUnitPriceComponentDto dto);
        Task EditAsync(EditUnitPriceComponentDto dto);
        Task DeleteAsync(int unitId, int id);
    }
}

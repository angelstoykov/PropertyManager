using PropertyManager.Application.DTOs.Unit;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IUnitsService
    {
        Task<int> CreateAsync(CreateUnitDto dto);

        Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query);

        Task EditAsync(EditUnitDto dto);

        Task DeleteAsync(int id);
    }
}

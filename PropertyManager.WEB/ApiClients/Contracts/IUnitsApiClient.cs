using PropertyManager.Application.DTOs.Unit;
using PropertyManager.WEB.ViewModels.Units;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IUnitsApiClient
    {
        Task<IEnumerable<UnitListViewModel>> GetAllAsync();

        Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query);

        Task<UnitDto> GetUnitByIdAsync(int id);

        Task CreateUnitAsync(CreateUnitDto model);

        Task DeleteUnitById(int id);

        Task<HttpResponseMessage> UpdateUnitAsync(EditUnitDto model);
    }
}

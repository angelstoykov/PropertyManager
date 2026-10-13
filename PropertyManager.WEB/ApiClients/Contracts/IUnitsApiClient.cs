using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.DTOs.UnitPriceComponents;
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

        Task<IEnumerable<UnitListItemDto>> GetUnitsByPropertyIdAsync(int propertyId);

        Task<IReadOnlyList<UnitPriceComponentDto>> GetPriceComponentsAsync(int unitId);

        Task<UnitPriceComponentDto?> GetPriceComponentByIdAsync(int unitId, int id);

        Task CreatePriceComponentAsync(CreateUnitPriceComponentDto dto);

        Task<HttpResponseMessage> UpdatePriceComponentAsync(int unitId, EditUnitPriceComponentDto dto);

        Task DeletePriceComponentAsync(int unitId, int id);
    }
}

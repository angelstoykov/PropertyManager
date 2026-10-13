using PropertyManager.Application.DTOs.PriceComponents;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IPriceComponentsApiClient
    {
        Task<IReadOnlyList<PriceComponentDto>> GetAllAsync();
        Task<PriceComponentDto?> GetByIdAsync(int id);
        Task CreateAsync(CreatePriceComponentDto dto);
        Task<HttpResponseMessage> UpdateAsync(EditPriceComponentDto dto);
        Task DeleteAsync(int id);
    }
}

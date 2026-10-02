using PropertyManager.Application.DTOs.Currencies;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface ICurrencyApiClient
    {
        Task<IEnumerable<CurrencyDto>> GetAllAsync();
        Task<IEnumerable<CurrencyDto>> GetAllActiveAsync();
    }
}

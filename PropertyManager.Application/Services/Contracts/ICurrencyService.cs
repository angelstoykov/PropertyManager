using PropertyManager.Application.DTOs.Currencies;

namespace PropertyManager.Application.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<IReadOnlyList<CurrencyDto>> GetAllAsync();
    }
}

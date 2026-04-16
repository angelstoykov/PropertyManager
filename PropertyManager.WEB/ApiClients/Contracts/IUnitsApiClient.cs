using PropertyManager.WEB.ViewModels.Units;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IUnitsApiClient
    {
        Task<IEnumerable<UnitListViewModel>> GetAllAsync();
    }
}

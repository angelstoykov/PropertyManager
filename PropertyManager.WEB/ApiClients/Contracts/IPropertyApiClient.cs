using PropertyManager.WEB.ViewModels.Properties;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IPropertyApiClient
    {
        Task<IEnumerable<PropertyListItemViewModel>> GetAllAsync();
    }
}

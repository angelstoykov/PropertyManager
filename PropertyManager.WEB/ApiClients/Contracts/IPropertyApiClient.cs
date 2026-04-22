using PropertyManager.Application.DTOs.Properties;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IPropertyApiClient
    {
        Task<IEnumerable<PropertyListItemDto>> GetAllAsync();
    }
}

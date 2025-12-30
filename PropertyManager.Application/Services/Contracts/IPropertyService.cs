using PropertyManager.Application.DTOs.Properties;

namespace PropertyManager.Application.Services.Contracts;

public interface IPropertyService
{
    Task<IEnumerable<PropertyListItemDto>> GetAllAsync();
}
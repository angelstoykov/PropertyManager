using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientRentedPropertiesViewModel
    {
        public int ClientId { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;
        public ClientType ClientType { get; set; }
        public int? PropertyIdToAdd { get; set; }
        public IReadOnlyList<ClientRentedPropertyDto> RentedProperties { get; set; } = Array.Empty<ClientRentedPropertyDto>();
        public IReadOnlyList<PropertyListItemDto> AvailableProperties { get; set; } = Array.Empty<PropertyListItemDto>();
    }
}

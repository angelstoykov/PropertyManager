using PropertyManager.Application.DTOs.Clients;

namespace PropertyManager.WEB.ApiClients.Contracts
{
    public interface IClientsApiClient
    {
        Task<IReadOnlyList<ClientListItemDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateClientDto dto);
        Task<HttpResponseMessage> UpdateAsync(EditClientDto dto);
        Task DeleteAsync(int id);
    }
}

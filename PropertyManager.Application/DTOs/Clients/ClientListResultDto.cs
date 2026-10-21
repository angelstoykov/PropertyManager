namespace PropertyManager.Application.DTOs.Clients
{
    public class ClientListResultDto
    {
        public IReadOnlyList<ClientListItemDto> Clients { get; set; } = [];
        public int AllCount { get; set; }
        public int OwnerCount { get; set; }
        public int TenantCount { get; set; }
        public int ContractorCount { get; set; }
    }
}

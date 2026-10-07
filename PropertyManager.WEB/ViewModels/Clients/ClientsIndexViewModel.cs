namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientsIndexViewModel
    {
        public string? Search { get; set; }
        public IReadOnlyList<ClientListItemViewModel> Clients { get; set; } = [];
    }
}

using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class DeleteClientViewModel
    {
        public int Id { get; set; }
        public ClientType ClientType { get; set; }
        public string DisplayName { get; set; } = null!;
    }
}

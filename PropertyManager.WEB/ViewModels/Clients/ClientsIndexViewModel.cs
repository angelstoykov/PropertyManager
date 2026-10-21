using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientsIndexViewModel
    {
        public string Tab { get; set; } = "all";
        public string? Search { get; set; }
        public bool SearchAll { get; set; }
        public string? Status { get; set; }
        public ClientType? ClientType { get; set; }
        public int? BuildingId { get; set; }
        public ClientRoleType? CreateRole { get; set; }
        public bool AccessDenied { get; set; }
        public IReadOnlyList<string> Columns { get; set; } = [];
        public IReadOnlyList<ClientTabViewModel> Tabs { get; set; } = [];
        public IReadOnlyList<SelectListItem> Buildings { get; set; } = [];
        public IReadOnlyList<ClientListItemViewModel> Clients { get; set; } = [];
    }

    public class ClientTabViewModel
    {
        public string Key { get; set; } = "all";
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

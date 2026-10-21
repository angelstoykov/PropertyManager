namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientRoleBadgeViewModel
    {
        public string Label { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CssClass { get; set; } = "bg-secondary";
    }
}

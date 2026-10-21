namespace PropertyManager.WEB.ViewModels.Clients
{
    public class FinancialRecordSubtypeOption
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
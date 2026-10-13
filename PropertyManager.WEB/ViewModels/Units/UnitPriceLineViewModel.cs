namespace PropertyManager.WEB.ViewModels.Units
{
    public class UnitPriceLineViewModel
    {
        public int Id { get; set; }
        public string ComponentName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; } = null!;
        public bool IncludeInCommission { get; set; }
    }

    public class UnitPriceTotalViewModel
    {
        public string CurrencySymbol { get; set; } = null!;
        public decimal TenantTotal { get; set; }
        public decimal CommissionBase { get; set; }
    }
}

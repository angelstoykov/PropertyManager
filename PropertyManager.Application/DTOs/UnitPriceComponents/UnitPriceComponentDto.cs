namespace PropertyManager.Application.DTOs.UnitPriceComponents
{
    public class UnitPriceComponentDto
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public int PriceComponentId { get; set; }
        public string PriceComponentName { get; set; } = null!;
        public bool IncludeInCommission { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public string CurrencySymbol { get; set; } = null!;
    }
}

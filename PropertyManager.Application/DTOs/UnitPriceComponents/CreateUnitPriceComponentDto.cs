namespace PropertyManager.Application.DTOs.UnitPriceComponents
{
    public class CreateUnitPriceComponentDto
    {
        public int UnitId { get; set; }
        public int PriceComponentId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}

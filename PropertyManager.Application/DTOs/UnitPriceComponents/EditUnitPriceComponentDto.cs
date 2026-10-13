namespace PropertyManager.Application.DTOs.UnitPriceComponents
{
    public class EditUnitPriceComponentDto
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}

namespace PropertyManager.Application.DTOs.PriceComponents
{
    public class CreatePriceComponentDto
    {
        public string Name { get; set; } = null!;
        public bool IncludeInCommission { get; set; }
    }
}

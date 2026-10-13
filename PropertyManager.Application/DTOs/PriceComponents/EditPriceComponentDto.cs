namespace PropertyManager.Application.DTOs.PriceComponents
{
    public class EditPriceComponentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IncludeInCommission { get; set; }
        public bool IsActive { get; set; }
    }
}

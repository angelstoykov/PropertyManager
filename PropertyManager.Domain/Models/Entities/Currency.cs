using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Models.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;       // EUR
        public string Name { get; set; } = null!;       // Euro
        public string? Symbol { get; set; }             // €
        public byte DecimalPlaces { get; set; } = 2;
        public bool IsActive { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
{
    public class PriceComponent
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IncludeInCommission { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<UnitPriceComponent> UnitPrices { get; set; } = new List<UnitPriceComponent>();
    }
}

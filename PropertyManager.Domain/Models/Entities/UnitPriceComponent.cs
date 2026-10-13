using Microsoft.EntityFrameworkCore;

namespace PropertyManager.Domain.Models.Entities
{
    public class UnitPriceComponent
    {
        public int Id { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public int PriceComponentId { get; set; }
        public PriceComponent PriceComponent { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
    }
}

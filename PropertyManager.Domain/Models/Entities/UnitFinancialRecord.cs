using Microsoft.EntityFrameworkCore;
using PropertyManager.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
{
    public class UnitFinancialRecord
    {
        [Key]
        public int Id { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public FinancialRecordType Type { get; set; }

        public DateTime Date { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}

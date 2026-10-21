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

        public int SubtypeId { get; set; }
        public FinancialRecordSubtype Subtype { get; set; } = null!;

        public DateTime Date { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public int? ContractorClientId { get; set; }
        public Client? ContractorClient { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}

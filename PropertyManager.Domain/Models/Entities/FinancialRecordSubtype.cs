using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Models.Entities
{
    public class FinancialRecordSubtype
    {
        public int Id { get; set; }

        public FinancialRecordType Type { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }

        public ICollection<UnitFinancialRecord> FinancialRecords { get; set; } = new List<UnitFinancialRecord>();
    }
}

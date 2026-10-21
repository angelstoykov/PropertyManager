using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.FinancialRecordSubtypes
{
    public class FinancialRecordSubtypeDto
    {
        public int Id { get; set; }
        public FinancialRecordType Type { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}

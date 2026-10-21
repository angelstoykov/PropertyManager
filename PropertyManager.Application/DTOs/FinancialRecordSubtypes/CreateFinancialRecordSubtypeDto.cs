using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.FinancialRecordSubtypes
{
    public class CreateFinancialRecordSubtypeDto
    {
        public FinancialRecordType Type { get; set; }
        public string Name { get; set; } = null!;
    }
}

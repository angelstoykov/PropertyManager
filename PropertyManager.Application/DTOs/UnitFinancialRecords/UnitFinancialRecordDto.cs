using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.UnitFinancialRecords
{
    public class UnitFinancialRecordDto
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public FinancialRecordType Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string? Comment { get; set; }
    }
}

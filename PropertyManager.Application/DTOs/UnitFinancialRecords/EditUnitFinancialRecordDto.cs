using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.UnitFinancialRecords
{
    public class EditUnitFinancialRecordDto
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public FinancialRecordType Type { get; set; }
        public int SubtypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public int? ContractorClientId { get; set; }
        public string? Comment { get; set; }
    }
}

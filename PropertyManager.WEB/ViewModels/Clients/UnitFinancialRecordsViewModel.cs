using PropertyManager.Application.DTOs.UnitFinancialRecords;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class UnitFinancialRecordsViewModel
    {
        public int ClientId { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitAddress { get; set; } = string.Empty;
        public UnitFinancialRecordFormViewModel NewRecord { get; set; } = new();
        public IReadOnlyList<UnitFinancialRecordDto> Records { get; set; } = Array.Empty<UnitFinancialRecordDto>();
    }
}

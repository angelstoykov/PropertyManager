using PropertyManager.Application.DTOs.FinancialRecordSubtypes;

namespace PropertyManager.WEB.ViewModels.FinancialRecordSubtypes
{
    public class FinancialRecordSubtypeIndexViewModel
    {
        public IReadOnlyList<FinancialRecordSubtypeDto> Subtypes { get; set; } = Array.Empty<FinancialRecordSubtypeDto>();
        public FinancialRecordSubtypeFormViewModel Form { get; set; } = new();
    }
}

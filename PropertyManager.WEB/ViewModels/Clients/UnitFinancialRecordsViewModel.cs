using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class UnitFinancialRecordsViewModel
    {
        public int ClientId { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitAddress { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public UnitFinancialRecordFormViewModel FinancialRecord { get; set; } = new();
        public IReadOnlyList<UnitFinancialRecordDto> Records { get; set; } = Array.Empty<UnitFinancialRecordDto>();
        public IReadOnlyList<SelectListItem> Currencies => FinancialRecord.Currencies;
        public IReadOnlyDictionary<int, string> CurrencySymbols { get; set; } =
            new Dictionary<int, string>();

        public async Task LoadActiveCurrenciesAsync(ICurrencyApiClient currencyApiClient)
        {
            await FinancialRecord.LoadActiveCurrenciesAsync(currencyApiClient);

            CurrencySymbols = FinancialRecord.Currencies
                .Where(item => int.TryParse(item.Value, out _))
                .ToDictionary(item => int.Parse(item.Value!), item => item.Text);
        }
    }
}

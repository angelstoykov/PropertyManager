using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ViewModels.Units
{
    public class UnitPriceFormViewModel
    {
        public int Id { get; set; }
        public int UnitId { get; set; }

        public string? ComponentName { get; set; }
        public bool IncludeInCommission { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a price component.")]
        [Display(Name = "Component")]
        public int PriceComponentId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a currency.")]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }

        public IReadOnlyList<SelectListItem> Components { get; set; } = Array.Empty<SelectListItem>();
        public IReadOnlyList<SelectListItem> Currencies { get; set; } = Array.Empty<SelectListItem>();

        public async Task LoadActiveCurrenciesAsync(ICurrencyApiClient currencyApiClient)
        {
            var active = (await currencyApiClient.GetAllActiveAsync()).ToList();

            Currencies = active
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(c.Symbol) ? c.Code : $"{c.Code} ({c.Symbol})"
                })
                .ToList();

            if (CurrencyId == 0 && active.Count > 0)
                CurrencyId = active[0].Id;
        }

        public void EnsureCurrencyOption(int currencyId, string text)
        {
            if (currencyId <= 0 || Currencies.Any(c => c.Value == currencyId.ToString()))
                return;

            Currencies = Currencies
                .Append(new SelectListItem { Value = currencyId.ToString(), Text = text })
                .ToList();
        }
    }
}

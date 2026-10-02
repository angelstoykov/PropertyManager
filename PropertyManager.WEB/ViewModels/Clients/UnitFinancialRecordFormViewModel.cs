using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Domain.Models.Enums;
using PropertyManager.WEB.ApiClients.Contracts;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class UnitFinancialRecordFormViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public FinancialRecordType Type { get; set; } = FinancialRecordType.Income;

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a currency.")]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        public IReadOnlyList<SelectListItem> Currencies { get; set; } = Array.Empty<SelectListItem>();

        public async Task LoadActiveCurrenciesAsync(ICurrencyApiClient currencyApiClient)
        {
            var active = (await currencyApiClient.GetAllActiveAsync()).ToList();

            Currencies = active
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(c.Symbol) ? c.Code : c.Symbol
                })
                .ToList();

            if (CurrencyId == 0 && active.Count > 0)
                CurrencyId = active[0].Id;
        }
    }
}

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

        [Required(ErrorMessage = "Select a subtype.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a subtype.")]
        [Display(Name = "Subtype")]
        public int? SubtypeId { get; set; }

        public int? FilterSubtypeId { get; set; }

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

        [Display(Name = "Contractor")]
        public int? ContractorClientId { get; set; }

        public IReadOnlyList<SelectListItem> Contractors { get; set; } = Array.Empty<SelectListItem>();

        public IReadOnlyList<SelectListItem> Currencies { get; set; } = Array.Empty<SelectListItem>();

        public IReadOnlyList<FinancialRecordSubtypeOption> SubtypeOptions { get; set; } = Array.Empty<FinancialRecordSubtypeOption>();

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

        public async Task LoadContractorsAsync(IClientsApiClient clientsApiClient)
        {
            var clients = await clientsApiClient.GetAllAsync(new PropertyManager.Application.DTOs.Clients.ClientListQuery { Tab = "all" });
            Contractors = clients.Clients
                .OrderBy(client => client.DisplayName)
                .Select(client => new SelectListItem(client.DisplayName, client.Id.ToString()))
                .ToList();
        }

        public async Task LoadSubtypesAsync(IFinancialRecordSubtypesApiClient subtypesApiClient)
        {
            var subtypes = await subtypesApiClient.GetAllAsync();
            SubtypeOptions = subtypes
                .Select(subtype => new FinancialRecordSubtypeOption
                {
                    Id = subtype.Id,
                    Type = (int)subtype.Type,
                    Name = subtype.Name,
                    IsActive = subtype.IsActive
                })
                .ToList();
        }
    }
}

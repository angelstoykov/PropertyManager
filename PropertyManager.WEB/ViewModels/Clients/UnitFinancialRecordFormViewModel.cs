using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Models.Enums;

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
        [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "Currency must be a 3-letter code.")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "BGN";

        [MaxLength(500)]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }
    }
}

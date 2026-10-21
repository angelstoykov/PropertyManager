using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.FinancialRecordSubtypes
{
    public class FinancialRecordSubtypeFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Type")]
        public FinancialRecordType Type { get; set; } = FinancialRecordType.Income;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}

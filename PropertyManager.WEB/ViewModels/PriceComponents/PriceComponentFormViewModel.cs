using System.ComponentModel.DataAnnotations;

namespace PropertyManager.WEB.ViewModels.PriceComponents
{
    public class PriceComponentFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Include in commission")]
        public bool IncludeInCommission { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}

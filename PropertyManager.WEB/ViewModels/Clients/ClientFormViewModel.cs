using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client type")]
        public ClientType ClientType { get; set; } = ClientType.Individual;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Display(Name = "Personal ID")]
        public string? PersonalId { get; set; }

        [Display(Name = "Company name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Company number")]
        public string? CompanyNumber { get; set; }

        [Display(Name = "VAT number")]
        public string? VatNumber { get; set; }

        [Display(Name = "Legal representative")]
        public string? LegalRepresentative { get; set; }
    }
}

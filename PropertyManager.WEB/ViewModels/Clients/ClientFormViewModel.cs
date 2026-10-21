using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Clients
{
    public class ClientFormViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client type")]
        public ClientType ClientType { get; set; } = ClientType.Individual;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Address { get; set; }

        [Display(Name = "Bank account (IBAN)")]
        [MaxLength(42)]
        public string? BankAccount { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "First name")]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(100)]
        public string? LastName { get; set; }

        [Display(Name = "Personal ID (ЕГН / foreign ID)")]
        [MaxLength(50)]
        public string? PersonalId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Company name")]
        [MaxLength(200)]
        public string? CompanyName { get; set; }

        [Display(Name = "Company ID (ЕИК / БУЛСТАТ)")]
        [MaxLength(50)]
        public string? CompanyNumber { get; set; }

        [Display(Name = "VAT number")]
        [MaxLength(50)]
        public string? VatNumber { get; set; }

        [Display(Name = "Legal representative (МОЛ)")]
        [MaxLength(200)]
        public string? LegalRepresentative { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ClientType == ClientType.Individual)
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    yield return new ValidationResult("First name is required.", [nameof(FirstName)]);
                if (string.IsNullOrWhiteSpace(LastName))
                    yield return new ValidationResult("Last name is required.", [nameof(LastName)]);
                if (string.IsNullOrWhiteSpace(PersonalId))
                    yield return new ValidationResult("Personal ID is required.", [nameof(PersonalId)]);
                if (DateOfBirth?.Date > DateTime.Today)
                    yield return new ValidationResult("Date of birth cannot be in the future.", [nameof(DateOfBirth)]);
            }
            else if (ClientType == ClientType.Company)
            {
                if (string.IsNullOrWhiteSpace(CompanyName))
                    yield return new ValidationResult("Company name is required.", [nameof(CompanyName)]);
                if (string.IsNullOrWhiteSpace(CompanyNumber))
                    yield return new ValidationResult("Company ID is required.", [nameof(CompanyNumber)]);
                if (string.IsNullOrWhiteSpace(LegalRepresentative))
                    yield return new ValidationResult("Legal representative is required.", [nameof(LegalRepresentative)]);
            }

            if (!string.IsNullOrWhiteSpace(BankAccount))
            {
                var iban = BankAccount.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant();
                if (iban.Length is < 15 or > 34 || !iban.All(char.IsLetterOrDigit) || !char.IsLetter(iban[0]) || !char.IsLetter(iban[1]))
                    yield return new ValidationResult("Bank account must be a valid IBAN.", [nameof(BankAccount)]);
            }
        }
    }
}

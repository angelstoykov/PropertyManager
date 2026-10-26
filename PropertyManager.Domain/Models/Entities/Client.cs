using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Models.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public ClientType ClientType { get; set; }

        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalId { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? VatNumber { get; set; }
        public string? LegalRepresentative { get; set; }

        public string? Address { get; set; }
        public string? BankAccount { get; set; }
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ClientUnit> ClientUnits { get; set; } = new List<ClientUnit>();
        public ICollection<ClientRole> Roles { get; set; } = new List<ClientRole>();
        public ICollection<UnitFinancialRecord> ContractorExpenses { get; set; } = new List<UnitFinancialRecord>();
        public ICollection<OwnershipRecord> OwnershipRecords { get; set; } = new List<OwnershipRecord>();
    }
}

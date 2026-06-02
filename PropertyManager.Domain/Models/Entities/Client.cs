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

        public string? CompanyName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? VatNumber { get; set; }
        public string? LegalRepresentative { get; set; }

        public ICollection<ClientUnit> ClientUnits { get; set; } = new List<ClientUnit>();
    }
}

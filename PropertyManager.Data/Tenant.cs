using PropertyManager.Data.Models.Entities;

namespace PropertyManager.Data
{
    public class Tenant
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // Optional ако е фирма
        public bool IsCompany { get; set; }
        public string? CompanyNumber { get; set; }

        public ICollection<Lease> Leases { get; set; } = new List<Lease>();
    }
}
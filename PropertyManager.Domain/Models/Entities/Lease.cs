using PropertyManager.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
{
    public class Lease
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }

        [Required]
        public LeaseStatus Status { get; set; }

        public ICollection<LeaseUnit> Units { get; set; } = new List<LeaseUnit>();
        public ICollection<RentPayment> Payments { get; set; } = new List<RentPayment>();
    }
}

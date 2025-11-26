using PropertyManager.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Models.Entities
{
    public class Lease
    {
        public int Id { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }

        [Required]
        public LeaseStatus Status { get; set; }
        public ICollection<RentPayment> Payments { get; set; } = new List<RentPayment>();
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Contracts;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Data.Models
{
    public class Unit : IUnit
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Type { get; set; }

        public string UnitNumber { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Area { get; set; }
        ////////

        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public int Floor { get; set; }

        public UnitStatus Status { get; set; }

        public ICollection<Lease> Leases { get; set; } = new List<Lease>();
    }
}
using Microsoft.EntityFrameworkCore;
using PropertyManager.Domain.Contracts;
using PropertyManager.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
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
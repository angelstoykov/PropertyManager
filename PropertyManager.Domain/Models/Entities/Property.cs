using PropertyManager.Domain.Contracts;
using PropertyManager.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
{
    public class Property : IProperty
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int NumberOfUnits { get; set; }
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public required string Owner { get; set; }
        public required PropertyType Type { get; set; }
        public required PropertyStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? Description { get; set; }
    }
}

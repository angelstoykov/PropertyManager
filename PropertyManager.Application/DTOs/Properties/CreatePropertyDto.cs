using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Properties
{
    public class CreatePropertyDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int NumberOfUnits { get; set; }
        //public IList<Unit> Units { get; set; } = new List<Unit>();
        public required string Owner { get; set; }
        public required PropertyType Type { get; set; }
        public required PropertyStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? Description { get; set; }
    }
}

using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Unit
{
    public class CreateUnitDto
    {
        public int PropertyId { get; set; }

        public string UnitNumber { get; set; } = null!;

        public int Floor { get; set; }

        public decimal SizeSqM { get; set; }

        public UnitStatus Status { get; set; }
    }

}

using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.DTOs.Properties
{
    public class PropertyListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public PropertyType Type { get; set; }
    }
}

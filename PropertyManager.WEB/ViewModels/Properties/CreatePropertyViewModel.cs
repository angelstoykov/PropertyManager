using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Properties
{
    public class CreatePropertyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public PropertyType Type { get; set; }
        public PropertyStatus Status { get; set; }
    }
}

using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Properties
{
    public class PropertyListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public PropertyType Type { get; set; }
    }
}

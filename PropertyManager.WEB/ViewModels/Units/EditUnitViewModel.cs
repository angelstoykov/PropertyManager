using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.WEB.ViewModels.Units
{
    public class EditUnitViewModel
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string UnitNumber { get; set; } = null!;

        public decimal Area { get; set; }

        public int PropertyId { get; set; }

        public int Floor { get; set; }

        public UnitStatus Status { get; set; }

        public List<SelectListItem> Properties { get; set; } = new();
    }
}

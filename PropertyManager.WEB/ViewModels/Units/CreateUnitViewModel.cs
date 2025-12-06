using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Domain.Models.Enums;

public class CreateUnitViewModel
{
    public int PropertyId { get; set; }

    public string UnitNumber { get; set; } = null!;

    public int Floor { get; set; }

    public decimal SizeSqM { get; set; }

    public UnitStatus Status { get; set; }

    public List<SelectListItem> Properties { get; set; } = new();
}

using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Domain.Models.Enums;

public class UnitIndexViewModel
{
    public List<UnitListItemDto> Units { get; set; } = new();

    public int? PropertyId { get; set; }
    public UnitStatus? Status { get; set; }

    public List<SelectListItem> Properties { get; set; } = new();

    public int Page { get; set; }
    public int TotalPages { get; set; }
}

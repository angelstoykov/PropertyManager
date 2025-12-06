using PropertyManager.Domain.Models.Enums;

public class UnitListItemDto
{
    public int Id { get; set; }
    public string UnitNumber { get; set; } = null!;
    public string PropertyName { get; set; } = null!;
    public int Floor { get; set; }
    public decimal Area { get; set; }
    public UnitStatus Status { get; set; }
}

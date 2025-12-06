using PropertyManager.Domain.Models.Enums;

public class UnitQueryDto
{
    public int? PropertyId { get; set; }
    public UnitStatus? Status { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

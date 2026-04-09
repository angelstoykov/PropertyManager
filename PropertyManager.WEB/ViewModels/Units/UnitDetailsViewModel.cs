public class UnitDetailsViewModel
{
    public int Id { get; set; }
    public string UnitNumber { get; set; } = null!;
    public decimal Area { get; set; }
    public int Floor { get; set; }
    public string Status { get; set; }

    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = null!;
}
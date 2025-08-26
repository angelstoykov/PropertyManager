using PropertyManager.Data.Models;

namespace PropertyManager.Data.Contracts
{
    internal interface IProperty
    {
        string Address { get; set; }
        string? Description { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        string? Notes { get; set; }
        int NumberOfUnits { get; set; }
        string Owner { get; set; }
        string PropertyType { get; set; }
        string Status { get; set; }
        IList<Unit> Units { get; set; }
    }
}
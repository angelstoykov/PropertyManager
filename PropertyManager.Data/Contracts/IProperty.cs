using PropertyManager.Data.Models.Entities;
using PropertyManager.Data.Models.Enums;

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
        PropertyType Type { get; set; }
        PropertyStatus Status { get; set; }
        IList<Unit> Units { get; set; }
    }
}
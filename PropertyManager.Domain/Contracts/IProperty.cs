using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Domain.Contracts
{
    public interface IProperty
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
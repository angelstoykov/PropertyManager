namespace PropertyManager.Domain.Models.Entities
{
    public class LeaseUnit
    {
        public int LeaseId { get; set; }
        public Lease Lease { get; set; } = null!;

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;
    }
}

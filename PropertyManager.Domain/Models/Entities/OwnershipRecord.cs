namespace PropertyManager.Domain.Models.Entities
{
    public class OwnershipRecord
    {
        public int Id { get; set; }

        public int? PropertyId { get; set; }
        public Property? Property { get; set; }

        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public decimal SharePercent { get; set; }

        public DateTime StartDate { get; set; }

        /// <summary>Exclusive end. Null while this share is open.</summary>
        public DateTime? EndDate { get; set; }
    }
}

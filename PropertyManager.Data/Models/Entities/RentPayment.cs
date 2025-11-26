using PropertyManager.Data.Models.Enums;

namespace PropertyManager.Data.Models.Entities
{
    public class RentPayment
    {
        public int Id { get; set; }

        public int LeaseId { get; set; }
        public Lease Lease { get; set; } = null!;

        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }

        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
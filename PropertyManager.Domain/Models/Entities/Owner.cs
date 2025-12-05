using PropertyManager.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Domain.Models.Entities
{
    internal class Owner : IOwner
    {
        [Key]
        public int Id { get; set; }
        public int OwnerType { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string BankAccount { get; set; }
    }
}

using PropertyManager.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Models.Entities
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

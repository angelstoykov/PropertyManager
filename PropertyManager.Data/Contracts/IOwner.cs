using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Contracts
{
    internal interface IOwner
    {
        public int Id { get; set; }
        public int OwnerType { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string BankAccount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyManager.Domain.Models.Entities
{
    public class ClientUnit
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}

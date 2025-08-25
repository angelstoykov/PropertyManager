using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Models
{
    internal class Property
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int NumberOfUnits { get; set; }
        public decimal MonthlyRent { get; set; }
        public DateTime DateAcquired { get; set; }
        public required string Owner { get; set; }
        public required string PropertyType { get; set; }
        public required string Status { get; set; }
        public string? Notes { get; set; }
        public string? Description { get; set; }
    }
}

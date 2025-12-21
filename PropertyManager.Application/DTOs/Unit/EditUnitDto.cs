using PropertyManager.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Application.DTOs.Unit
{
    public class EditUnitDto
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string UnitNumber { get; set; } = null!;

        public decimal Area { get; set; }

        public int PropertyId { get; set; }

        public int Floor { get; set; }

        public UnitStatus Status { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Contracts
{
    internal interface IUnit
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public decimal Area { get; set; }
    }
}

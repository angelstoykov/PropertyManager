using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Contracts;
using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Data.Models
{
    internal class Unit : IUnit
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Type { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Area { get; set; }
    }
}
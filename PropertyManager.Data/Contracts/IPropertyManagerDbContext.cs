using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Models;

namespace PropertyManager.Data.Contracts
{
    interface IPropertyManagerDbContext
    {
        public DbSet<Property> Properties { get; set; }
    }
}
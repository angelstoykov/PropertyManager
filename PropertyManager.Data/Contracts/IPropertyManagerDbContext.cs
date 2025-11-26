using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Models.Entities;

namespace PropertyManager.Data.Contracts
{
    interface IPropertyManagerDbContext
    {
        public DbSet<Property> Properties { get; }

        public DbSet<Unit> Units { get; }
    }
}
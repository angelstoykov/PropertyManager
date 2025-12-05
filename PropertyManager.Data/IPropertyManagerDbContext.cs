using Microsoft.EntityFrameworkCore;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Data
{
    public interface IPropertyManagerDbContext
    {
        public DbSet<Property> Properties { get; }

        public DbSet<Unit> Units { get; }
    }
}
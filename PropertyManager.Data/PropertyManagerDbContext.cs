using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Models;

namespace PropertyManager.Data
{
    public class PropertyManagerDbContext : DbContext
    {
        public PropertyManagerDbContext(DbContextOptions<PropertyManagerDbContext> options)
            : base(options)
        {   
        }

        DbSet<Property> Properties { get; set; }
        }
}

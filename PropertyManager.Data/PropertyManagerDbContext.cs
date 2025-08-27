using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Contracts;
using PropertyManager.Data.Models;

namespace PropertyManager.Data
{
    public class PropertyManagerDbContext : DbContext, IPropertyManagerDbContext
    {
        public PropertyManagerDbContext(DbContextOptions<PropertyManagerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Address).IsRequired();
                entity.Property(e => e.Owner).IsRequired();
                entity.Property(e => e.PropertyType).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Property> Properties { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Contracts;
using PropertyManager.Data.Models.Entities;
using System.Numerics;

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
            // Property entity configuration
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

            // Unit entity configuration
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(u => u.UnitNumber).IsRequired().HasMaxLength(50);

                entity.HasOne(u => u.Property)
                    .WithMany(p => p.Units)
                    .HasForeignKey(u => u.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Tenant
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.Property(t => t.FullName).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Email).HasMaxLength(200);
            });

            // Lease
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.Property(l => l.MonthlyRent).HasColumnType("decimal(18,2)");
                entity.Property(l => l.DepositAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.Unit)
                    .WithMany(u => u.Leases)
                    .HasForeignKey(l => l.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Tenant)
                    .WithMany(t => t.Leases)
                    .HasForeignKey(l => l.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RentPayment
            modelBuilder.Entity<RentPayment>(entity =>
            {
                entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(r => r.Lease)
                    .WithMany(l => l.Payments)
                    .HasForeignKey(r => r.LeaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Unit> Units => Set<Unit>();
        //public DbSet<Tenant> Tenants => Set<Tenant>();
        //public DbSet<Lease> Leases => Set<Lease>();
        //public DbSet<RentPayment> RentPayments => Set<RentPayment>();
        //public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        //public DbSet<Vendor> Vendors => Set<Vendor>();
    }
}

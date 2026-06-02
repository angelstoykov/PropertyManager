using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Identity;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Data
{
    public class PropertyManagerDbContext : IdentityDbContext<ApplicationUser>, IPropertyManagerDbContext
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
                entity.Property(e => e.Type).IsRequired();
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

                entity.HasMany(e => e.ClientUnits);
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

            // ----------------------------
            // TENANT
            // ----------------------------
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.Property(t => t.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Email)
                      .HasMaxLength(200);

                entity.Property(t => t.Phone)
                      .HasMaxLength(50);
            });

            // ----------------------------
            // LEASE
            // ----------------------------
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.Property(l => l.MonthlyRent)
                      .HasColumnType("decimal(18,2)");

                entity.Property(l => l.DepositAmount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.Unit)
                      .WithMany(u => u.Leases)
                      .HasForeignKey(l => l.UnitId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Tenant)
                      .WithMany(t => t.Leases)
                      .HasForeignKey(l => l.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------------------
            // ENUM към INT
            // ----------------------------
            modelBuilder.Entity<Unit>()
                .Property(u => u.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Lease>()
                .Property(l => l.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Phone).IsRequired().HasMaxLength(50);
                entity.Property(c => c.FirstName).HasMaxLength(100);
                entity.Property(c => c.LastName).HasMaxLength(100);
                entity.Property(c => c.PersonalId).HasMaxLength(20);
                entity.Property(c => c.CompanyName).HasMaxLength(200);
                entity.Property(c => c.CompanyNumber).HasMaxLength(50);
                entity.Property(c => c.VatNumber).HasMaxLength(50);
                entity.Property(c => c.LegalRepresentative).HasMaxLength(200);

                entity.Property(c => c.ClientType).HasConversion<int>();

                entity.HasMany(c => c.ClientUnits)
                      .WithOne(cu => cu.Client)
                      .HasForeignKey(cu => cu.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientUnit>()
                .HasKey(cu => new { cu.ClientId, cu.UnitId });

            modelBuilder.Entity<ClientUnit>()
                .HasOne(cu => cu.Client)
                .WithMany(c => c.ClientUnits)
                .HasForeignKey(cu => cu.ClientId);

            modelBuilder.Entity<ClientUnit>()
                .HasOne(cu => cu.Unit)
                .WithMany(u => u.ClientUnits)
                .HasForeignKey(cu => cu.UnitId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<RentPayment> RentPayments => Set<RentPayment>();
        public DbSet<Client> Clients => Set<Client>();
        //public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        //public DbSet<Vendor> Vendors => Set<Vendor>();
    }
}

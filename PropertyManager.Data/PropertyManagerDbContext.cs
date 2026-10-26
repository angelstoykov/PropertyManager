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

            // Lease
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.Property(l => l.MonthlyRent).HasColumnType("decimal(18,2)");
                entity.Property(l => l.DepositAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.Unit)
                    .WithMany(u => u.Leases)
                    .HasForeignKey(l => l.UnitId)
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
                entity.Property(c => c.PersonalId).HasMaxLength(50);
                entity.Property(c => c.DateOfBirth).HasColumnType("date");
                entity.Property(c => c.CompanyName).HasMaxLength(200);
                entity.Property(c => c.CompanyNumber).HasMaxLength(50);
                entity.Property(c => c.VatNumber).HasMaxLength(50);
                entity.Property(c => c.LegalRepresentative).HasMaxLength(200);
                entity.Property(c => c.Address).HasMaxLength(300);
                entity.Property(c => c.BankAccount).HasMaxLength(34);
                entity.Property(c => c.Notes).HasMaxLength(1000);
                entity.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

                entity.Property(c => c.ClientType).HasConversion<int>();

                entity.HasIndex(c => c.PersonalId)
                    .IsUnique()
                    .HasFilter("[PersonalId] IS NOT NULL");

                entity.HasIndex(c => c.CompanyNumber)
                    .IsUnique()
                    .HasFilter("[CompanyNumber] IS NOT NULL");

                entity.HasMany(c => c.ClientUnits)
                      .WithOne(cu => cu.Client)
                      .HasForeignKey(cu => cu.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientRole>(entity =>
            {
                entity.Property(r => r.Role).HasConversion<int>();
                entity.Property(r => r.StartDate).HasColumnType("date");
                entity.Property(r => r.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(r => new { r.ClientId, r.Role }).IsUnique();

                entity.HasOne(r => r.Client)
                    .WithMany(c => c.Roles)
                    .HasForeignKey(r => r.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OwnershipRecord>(entity =>
            {
                entity.Property(o => o.SharePercent).HasColumnType("decimal(5,2)");
                entity.Property(o => o.StartDate).HasColumnType("date");
                entity.Property(o => o.EndDate).HasColumnType("date");

                entity.ToTable(table => table.HasCheckConstraint(
                    "CK_OwnershipRecords_Target",
                    "([PropertyId] IS NOT NULL AND [UnitId] IS NULL) OR ([PropertyId] IS NULL AND [UnitId] IS NOT NULL)"));

                entity.HasOne(o => o.Property)
                    .WithMany()
                    .HasForeignKey(o => o.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Unit)
                    .WithMany()
                    .HasForeignKey(o => o.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Client)
                    .WithMany(c => c.OwnershipRecords)
                    .HasForeignKey(o => o.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.ClientId);
                entity.HasIndex(o => new { o.PropertyId, o.StartDate });
                entity.HasIndex(o => new { o.UnitId, o.StartDate });
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

            modelBuilder.Entity<UnitFinancialRecord>(entity =>
            {
                entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");
                entity.Property(r => r.Comment).HasMaxLength(500);
                entity.Property(r => r.Type).HasConversion<int>();

                entity.HasOne(r => r.Unit)
                    .WithMany(u => u.FinancialRecords)
                    .HasForeignKey(r => r.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Currency)
                    .WithMany()
                    .HasForeignKey(r => r.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Subtype)
                    .WithMany(s => s.FinancialRecords)
                    .HasForeignKey(r => r.SubtypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.ContractorClient)
                    .WithMany(c => c.ContractorExpenses)
                    .HasForeignKey(r => r.ContractorClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => new { r.UnitId, r.Date });
                entity.HasIndex(r => r.CurrencyId);
                entity.HasIndex(r => r.SubtypeId);
                entity.HasIndex(r => r.ContractorClientId);
            });

            modelBuilder.Entity<FinancialRecordSubtype>(entity =>
            {
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Type).HasConversion<int>();
                entity.Property(s => s.IsActive).IsRequired();
                entity.HasIndex(s => new { s.Type, s.Name }).IsUnique();
            });

            modelBuilder.Entity<PriceComponent>(entity =>
            {
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.Name).IsUnique();
                entity.Property(c => c.IncludeInCommission).IsRequired();
                entity.Property(c => c.IsActive).IsRequired();
            });

            modelBuilder.Entity<UnitPriceComponent>(entity =>
            {
                entity.Property(c => c.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.Unit)
                    .WithMany(u => u.PriceComponents)
                    .HasForeignKey(c => c.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.PriceComponent)
                    .WithMany(p => p.UnitPrices)
                    .HasForeignKey(c => c.PriceComponentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Currency)
                    .WithMany()
                    .HasForeignKey(c => c.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => new { c.UnitId, c.PriceComponentId }).IsUnique();
                entity.HasIndex(c => c.CurrencyId);
            });

            // ----------------------------
            // CURRENCY
            // ----------------------------
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Code).IsRequired().HasMaxLength(3);
                entity.HasIndex(r => r.Code).IsUnique();
                entity.Property(r => r.Name).IsRequired().HasMaxLength(20);
                entity.Property(r => r.IsActive).IsRequired();
                entity.Property(r => r.DecimalPlaces).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<RentPayment> RentPayments => Set<RentPayment>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<ClientRole> ClientRoles => Set<ClientRole>();
        public DbSet<OwnershipRecord> OwnershipRecords => Set<OwnershipRecord>();
        public DbSet<UnitFinancialRecord> UnitFinancialRecords => Set<UnitFinancialRecord>();
        public DbSet<FinancialRecordSubtype> FinancialRecordSubtypes => Set<FinancialRecordSubtype>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<PriceComponent> PriceComponents => Set<PriceComponent>();
        public DbSet<UnitPriceComponent> UnitPriceComponents => Set<UnitPriceComponent>();

        //public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        //public DbSet<Vendor> Vendors => Set<Vendor>();
    }
}

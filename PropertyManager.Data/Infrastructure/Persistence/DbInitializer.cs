using Microsoft.EntityFrameworkCore;
using PropertyManager.Data.Models.Entities;
using PropertyManager.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Data.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(PropertyManagerDbContext context)
        {
            // ✅ Автоматично прилага миграции
            //await context.Database.MigrateAsync();
            context.Database.MigrateAsync();

            // ✅ Ако вече има Properties → не seed-ваме повторно
            if (context.Properties.Any())
                return;

            // -----------------------------
            // 🏢 PROPERTIES
            // -----------------------------
            var property1 = new Property
            {
                Name = "Sunrise Apartments",
                Address = "бул. България 100",
                Type = PropertyType.Residential,
                Owner = "Георги Иванов",
                Status = PropertyStatus.Active
            };

            var property2 = new Property
            {
                Name = "Business Center Omega",
                Address = "ул. Цар Освободител 12",
                Type = PropertyType.Commercial,
                Owner = "Мария Петрова",
                Status = PropertyStatus.Inactive
            };

            context.Properties.AddRange(property1, property2);
            //await context.SaveChangesAsync();
            context.SaveChanges();

            // -----------------------------
            // 🏘 UNITS
            // -----------------------------
            var unit1 = new Unit
            {
                PropertyId = property1.Id,
                UnitNumber = "A1",
                Floor = 2,
                Area = 72,
                Status = UnitStatus.Occupied
            };

            var unit2 = new Unit
            {
                PropertyId = property1.Id,
                UnitNumber = "A2",
                Floor = 3,
                Area = 85,
                Status = UnitStatus.Vacant
            };

            var unit3 = new Unit
            {
                PropertyId = property2.Id,
                UnitNumber = "Office 301",
                Floor = 3,
                Area = 120,
                Status = UnitStatus.Occupied
            };

            context.Units.AddRange(unit1, unit2, unit3);
            //await context.SaveChangesAsync();
            context.SaveChanges();

            // -----------------------------
            // 👤 TENANTS
            // -----------------------------
            var tenant1 = new Tenant
            {
                FullName = "Иван Петров",
                Email = "ivan.petrov@mail.com",
                Phone = "0888123456",
                IsCompany = false
            };

            var tenant2 = new Tenant
            {
                FullName = "Tech Solutions Ltd.",
                Email = "office@techsolutions.bg",
                Phone = "02 900 2000",
                IsCompany = true,
                CompanyNumber = "BG204812345"
            };

            context.Tenants.AddRange(tenant1, tenant2);
            //await context.SaveChangesAsync();
            context.SaveChanges();

            // -----------------------------
            // 📄 LEASES
            // -----------------------------
            var lease1 = new Lease
            {
                UnitId = unit1.Id,
                TenantId = tenant1.Id,
                StartDate = DateTime.UtcNow.AddMonths(-3),
                MonthlyRent = 900,
                DepositAmount = 1800,
                Status = LeaseStatus.Active
            };

            var lease2 = new Lease
            {
                UnitId = unit3.Id,
                TenantId = tenant2.Id,
                StartDate = DateTime.UtcNow.AddYears(-1),
                EndDate = DateTime.UtcNow.AddMonths(-2),
                MonthlyRent = 2200,
                DepositAmount = 4400,
                Status = LeaseStatus.Expired
            };

            context.Leases.AddRange(lease1, lease2);
            //await context.SaveChangesAsync();
            context.SaveChangesAsync();

            // -----------------------------
            // 💰 PAYMENTS
            // -----------------------------
            context.RentPayments.AddRange(
                new RentPayment
                {
                    LeaseId = lease1.Id,
                    DueDate = DateTime.UtcNow.AddMonths(-1),
                    PaidDate = DateTime.UtcNow.AddMonths(-1).AddDays(2),
                    Amount = 900,
                    Status = PaymentStatus.Paid,
                    Method = PaymentMethod.BankTransfer
                },
                new RentPayment
                {
                    LeaseId = lease1.Id,
                    DueDate = DateTime.UtcNow,
                    Amount = 900,
                    Status = PaymentStatus.Pending,
                    Method = PaymentMethod.BankTransfer
                }
            );

            // -----------------------------
            // 🧰 VENDOR + MAINTENANCE
            // -----------------------------
            //var vendor = new Vendor
            //{
            //    Name = "FixIt Services",
            //    Phone = "0899112233",
            //    Email = "support@fixit.bg"
            //};

            //context.Vendors.Add(vendor);
            //await context.SaveChangesAsync();

            //context.MaintenanceRequests.Add(
            //    new MaintenanceRequest
            //    {
            //        UnitId = unit1.Id,
            //        Title = "Теч под мивката",
            //        Description = "Има теч в кухнята под мивката",
            //        Status = MaintenanceStatus.Open,
            //        AssignedVendorId = vendor.Id
            //    }
            //);

            context.SaveChanges();
        }
    }
}

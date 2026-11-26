using Microsoft.EntityFrameworkCore;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Data.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(PropertyManagerDbContext context)
        {
            // Автоматично прилага миграции
            await context.Database.MigrateAsync();

            if (!context.Currencies.Any())
            {
                var currency = new Currency
                {
                    Code = "EUR",
                    DecimalPlaces = 2,
                    IsActive = true,
                    Name = "Euro",
                    Symbol = "€"
                };

                context.Currencies.Add(currency);
            }

            if (!context.PriceComponents.Any())
            {
                context.PriceComponents.AddRange(
                    new PriceComponent
                    {
                        Name = "Rental price",
                        IncludeInCommission = true,
                        IsActive = true
                    },
                    new PriceComponent
                    {
                        Name = "Maintenance fee",
                        IncludeInCommission = false,
                        IsActive = true
                    });
            }

            context.SaveChanges();
        }
    }
}

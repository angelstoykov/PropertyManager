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

            // Проверява за записи. Ако липсват хвърля грешка.
            if (!context.Properties.Any())
                throw new Exception("There is no properties in the DB.");

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

            context.SaveChanges();
        }
    }
}

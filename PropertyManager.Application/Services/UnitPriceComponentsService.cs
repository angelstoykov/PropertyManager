using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.UnitPriceComponents;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Application.Services
{
    public class UnitPriceComponentsService : IUnitPriceComponentsService
    {
        private readonly PropertyManagerDbContext _context;

        public UnitPriceComponentsService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UnitPriceComponentDto>> GetByUnitAsync(int unitId)
        {
            await EnsureUnitExistsAsync(unitId);

            return await _context.UnitPriceComponents
                .AsNoTracking()
                .Where(line => line.UnitId == unitId)
                .OrderBy(line => line.PriceComponent.Name)
                .Select(line => new UnitPriceComponentDto
                {
                    Id = line.Id,
                    UnitId = line.UnitId,
                    PriceComponentId = line.PriceComponentId,
                    PriceComponentName = line.PriceComponent.Name,
                    IncludeInCommission = line.PriceComponent.IncludeInCommission,
                    Amount = line.Amount,
                    CurrencyId = line.CurrencyId,
                    CurrencyCode = line.Currency.Code,
                    CurrencySymbol = line.Currency.Symbol ?? line.Currency.Code
                })
                .ToListAsync();
        }

        public async Task<UnitPriceComponentDto?> GetByIdAsync(int unitId, int id)
        {
            await EnsureUnitExistsAsync(unitId);

            var line = await _context.UnitPriceComponents
                .AsNoTracking()
                .Include(l => l.PriceComponent)
                .Include(l => l.Currency)
                .FirstOrDefaultAsync(l => l.Id == id && l.UnitId == unitId);

            return line == null ? null : MapToDto(line);
        }

        public async Task<int> CreateAsync(CreateUnitPriceComponentDto dto)
        {
            await EnsureUnitExistsAsync(dto.UnitId);
            await ValidateAmountAndCurrencyAsync(dto.Amount, dto.CurrencyId, requireActiveCurrency: true);
            await EnsureComponentCanBeAssignedAsync(dto.UnitId, dto.PriceComponentId);

            var line = new UnitPriceComponent
            {
                UnitId = dto.UnitId,
                PriceComponentId = dto.PriceComponentId,
                Amount = dto.Amount,
                CurrencyId = dto.CurrencyId
            };

            _context.UnitPriceComponents.Add(line);
            await _context.SaveChangesAsync();

            return line.Id;
        }

        public async Task EditAsync(EditUnitPriceComponentDto dto)
        {
            await EnsureUnitExistsAsync(dto.UnitId);

            var line = await _context.UnitPriceComponents
                .FirstOrDefaultAsync(l => l.Id == dto.Id && l.UnitId == dto.UnitId);

            if (line == null)
                throw new InvalidOperationException("Unit price component not found.");

            var requireActiveCurrency = line.CurrencyId != dto.CurrencyId;
            await ValidateAmountAndCurrencyAsync(dto.Amount, dto.CurrencyId, requireActiveCurrency);

            line.Amount = dto.Amount;
            line.CurrencyId = dto.CurrencyId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int unitId, int id)
        {
            await EnsureUnitExistsAsync(unitId);

            var line = await _context.UnitPriceComponents
                .FirstOrDefaultAsync(l => l.Id == id && l.UnitId == unitId);

            if (line == null)
                return;

            _context.UnitPriceComponents.Remove(line);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureUnitExistsAsync(int unitId)
        {
            var exists = await _context.Units.AnyAsync(u => u.Id == unitId);
            if (!exists)
                throw new InvalidOperationException("Unit not found.");
        }

        private async Task EnsureComponentCanBeAssignedAsync(int unitId, int priceComponentId)
        {
            var component = await _context.PriceComponents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == priceComponentId);

            if (component == null || !component.IsActive)
                throw new InvalidOperationException("Price component is not available.");

            var alreadyAssigned = await _context.UnitPriceComponents
                .AnyAsync(line => line.UnitId == unitId && line.PriceComponentId == priceComponentId);

            if (alreadyAssigned)
                throw new InvalidOperationException("This price component is already on the unit.");
        }

        private async Task ValidateAmountAndCurrencyAsync(decimal amount, int currencyId, bool requireActiveCurrency)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero.");

            var currencyQuery = _context.Currencies.AsNoTracking().Where(c => c.Id == currencyId);
            if (requireActiveCurrency)
                currencyQuery = currencyQuery.Where(c => c.IsActive);

            var exists = await currencyQuery.AnyAsync();
            if (!exists)
                throw new InvalidOperationException("Currency is not valid.");
        }

        private static UnitPriceComponentDto MapToDto(UnitPriceComponent line) => new()
        {
            Id = line.Id,
            UnitId = line.UnitId,
            PriceComponentId = line.PriceComponentId,
            PriceComponentName = line.PriceComponent.Name,
            IncludeInCommission = line.PriceComponent.IncludeInCommission,
            Amount = line.Amount,
            CurrencyId = line.CurrencyId,
            CurrencyCode = line.Currency.Code,
            CurrencySymbol = line.Currency.Symbol ?? line.Currency.Code
        };
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Currencies;
using PropertyManager.Data;

namespace PropertyManager.Application.Services.Contracts
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IPropertyManagerDbContext _context;

        public CurrencyService(IPropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CurrencyDto>> GetAllAsync()
        {
            return await _context.Currencies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    DecimalPlaces = c.DecimalPlaces,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<CurrencyDto>> GetAllActiveAsync()
        {
            return await _context.Currencies
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    DecimalPlaces = c.DecimalPlaces,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Application.Services
{
    public class UnitFinancialRecordsService : IUnitFinancialRecordsService
    {
        private readonly PropertyManagerDbContext _context;

        public UnitFinancialRecordsService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UnitFinancialRecordDto>> GetByClientAndUnitAsync(int clientId, int unitId)
        {
            await EnsureClientHasUnitAsync(clientId, unitId);

            return await _context.UnitFinancialRecords
                .AsNoTracking()
                .Where(r => r.UnitId == unitId)
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Id)
                .Select(r => new UnitFinancialRecordDto
                {
                    Id = r.Id,
                    UnitId = r.UnitId,
                    Type = r.Type,
                    Date = r.Date,
                    Amount = r.Amount,
                    CurrencyId = r.CurrencyId,
                    CurrencyCode = r.Currency.Code,
                    CurrencySymbol = r.Currency.Symbol ?? r.Currency.Code,
                    Comment = r.Comment
                })
                .ToListAsync();
        }

        public async Task<UnitFinancialRecordDto?> GetByIdAsync(int clientId, int unitId, int id)
        {
            await EnsureClientHasUnitAsync(clientId, unitId);

            var record = await _context.UnitFinancialRecords
                .AsNoTracking()
                .Include(r => r.Currency)
                .FirstOrDefaultAsync(r => r.Id == id && r.UnitId == unitId);

            return record == null ? null : MapToDto(record);
        }

        public async Task<int> CreateAsync(int clientId, CreateUnitFinancialRecordDto dto)
        {
            await EnsureClientHasUnitAsync(clientId, dto.UnitId);
            await ValidateRecordFieldsAsync(dto.Amount, dto.CurrencyId, requireActive: true);

            var record = new UnitFinancialRecord
            {
                UnitId = dto.UnitId,
                Type = dto.Type,
                Date = dto.Date.Date,
                Amount = dto.Amount,
                CurrencyId = dto.CurrencyId,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim()
            };

            _context.UnitFinancialRecords.Add(record);
            await _context.SaveChangesAsync();

            return record.Id;
        }

        public async Task EditAsync(int clientId, EditUnitFinancialRecordDto dto)
        {
            await EnsureClientHasUnitAsync(clientId, dto.UnitId);

            var record = await _context.UnitFinancialRecords
                .FirstOrDefaultAsync(r => r.Id == dto.Id && r.UnitId == dto.UnitId);

            if (record == null)
                throw new InvalidOperationException("Financial record not found.");

            var requireActive = record.CurrencyId != dto.CurrencyId;
            await ValidateRecordFieldsAsync(dto.Amount, dto.CurrencyId, requireActive);

            record.Type = dto.Type;
            record.Date = dto.Date.Date;
            record.Amount = dto.Amount;
            record.CurrencyId = dto.CurrencyId;
            record.Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int clientId, int unitId, int id)
        {
            await EnsureClientHasUnitAsync(clientId, unitId);

            var record = await _context.UnitFinancialRecords
                .FirstOrDefaultAsync(r => r.Id == id && r.UnitId == unitId);

            if (record == null)
                return;

            _context.UnitFinancialRecords.Remove(record);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureClientHasUnitAsync(int clientId, int unitId)
        {
            var hasUnit = await _context.Clients
                .AsNoTracking()
                .Where(c => c.Id == clientId)
                .SelectMany(c => c.ClientUnits)
                .AnyAsync(cu => cu.UnitId == unitId);

            if (!hasUnit)
                throw new InvalidOperationException("Unit is not rented by this client.");
        }

        private async Task ValidateRecordFieldsAsync(decimal amount, int currencyId, bool requireActive)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero.");

            var currencyQuery = _context.Currencies.AsNoTracking().Where(c => c.Id == currencyId);
            if (requireActive)
                currencyQuery = currencyQuery.Where(c => c.IsActive);

            var exists = await currencyQuery.AnyAsync();
            if (!exists)
                throw new InvalidOperationException("Currency is not valid.");
        }

        private static UnitFinancialRecordDto MapToDto(UnitFinancialRecord record) => new()
        {
            Id = record.Id,
            UnitId = record.UnitId,
            Type = record.Type,
            Date = record.Date,
            Amount = record.Amount,
            CurrencyId = record.CurrencyId,
            CurrencyCode = record.Currency.Code,
            CurrencySymbol = record.Currency.Symbol ?? record.Currency.Code,
            Comment = record.Comment
        };
    }
}

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
                    Currency = r.Currency,
                    Comment = r.Comment
                })
                .ToListAsync();
        }

        public async Task<UnitFinancialRecordDto?> GetByIdAsync(int clientId, int unitId, int id)
        {
            await EnsureClientHasUnitAsync(clientId, unitId);

            var record = await _context.UnitFinancialRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.UnitId == unitId);

            return record == null ? null : MapToDto(record);
        }

        public async Task<int> CreateAsync(int clientId, CreateUnitFinancialRecordDto dto)
        {
            await EnsureClientHasUnitAsync(clientId, dto.UnitId);
            ValidateRecordFields(dto.Amount, dto.Currency);

            var record = new UnitFinancialRecord
            {
                UnitId = dto.UnitId,
                Type = dto.Type,
                Date = dto.Date.Date,
                Amount = dto.Amount,
                Currency = dto.Currency.Trim().ToUpperInvariant(),
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim()
            };

            _context.UnitFinancialRecords.Add(record);
            await _context.SaveChangesAsync();

            return record.Id;
        }

        public async Task EditAsync(int clientId, EditUnitFinancialRecordDto dto)
        {
            await EnsureClientHasUnitAsync(clientId, dto.UnitId);
            ValidateRecordFields(dto.Amount, dto.Currency);

            var record = await _context.UnitFinancialRecords
                .FirstOrDefaultAsync(r => r.Id == dto.Id && r.UnitId == dto.UnitId);

            if (record == null)
                throw new InvalidOperationException("Financial record not found.");

            record.Type = dto.Type;
            record.Date = dto.Date.Date;
            record.Amount = dto.Amount;
            record.Currency = dto.Currency.Trim().ToUpperInvariant();
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

        private static void ValidateRecordFields(decimal amount, string currency)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new InvalidOperationException("Currency must be a 3-letter ISO code.");
        }

        private static UnitFinancialRecordDto MapToDto(UnitFinancialRecord record) => new()
        {
            Id = record.Id,
            UnitId = record.UnitId,
            Type = record.Type,
            Date = record.Date,
            Amount = record.Amount,
            Currency = record.Currency,
            Comment = record.Comment
        };
    }
}

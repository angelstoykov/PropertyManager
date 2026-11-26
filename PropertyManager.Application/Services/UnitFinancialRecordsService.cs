using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.UnitFinancialRecords;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services
{
    public class UnitFinancialRecordsService : IUnitFinancialRecordsService
    {
        private readonly PropertyManagerDbContext _context;
        private readonly IClientsService _clientsService;

        public UnitFinancialRecordsService(PropertyManagerDbContext context, IClientsService clientsService)
        {
            _context = context;
            _clientsService = clientsService;
        }

        public async Task<IReadOnlyList<UnitFinancialRecordDto>> GetByClientAndUnitAsync(int clientId, int unitId, int? subtypeId = null)
        {
            await EnsureClientHasUnitAsync(clientId, unitId);

            var query = _context.UnitFinancialRecords
                .AsNoTracking()
                .Where(r => r.UnitId == unitId);

            if (subtypeId is > 0)
                query = query.Where(r => r.SubtypeId == subtypeId.Value);

            return await query
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Id)
                .Select(r => new UnitFinancialRecordDto
                {
                    Id = r.Id,
                    UnitId = r.UnitId,
                    Type = r.Type,
                    SubtypeId = r.SubtypeId,
                    SubtypeName = r.Subtype.Name,
                    SubtypeIsActive = r.Subtype.IsActive,
                    Date = r.Date,
                    Amount = r.Amount,
                    CurrencyId = r.CurrencyId,
                    CurrencyCode = r.Currency.Code,
                    CurrencySymbol = r.Currency.Symbol ?? r.Currency.Code,
                    ContractorClientId = r.ContractorClientId,
                    ContractorName = r.ContractorClient == null
                        ? null
                        : r.ContractorClient.ClientType == ClientType.Company
                            ? r.ContractorClient.CompanyName
                            : r.ContractorClient.FirstName + " " + r.ContractorClient.LastName,
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
                .Include(r => r.Subtype)
                .Include(r => r.ContractorClient)
                .FirstOrDefaultAsync(r => r.Id == id && r.UnitId == unitId);

            return record == null ? null : MapToDto(record);
        }

        public async Task<int> CreateAsync(int clientId, CreateUnitFinancialRecordDto dto)
        {
            await EnsureClientHasUnitAsync(clientId, dto.UnitId);
            await ValidateRecordFieldsAsync(dto.Amount, dto.CurrencyId, requireActive: true);
            await ValidateSubtypeAsync(dto.Type, dto.SubtypeId, requireActive: true);

            var record = new UnitFinancialRecord
            {
                UnitId = dto.UnitId,
                Type = dto.Type,
                SubtypeId = dto.SubtypeId,
                Date = dto.Date.Date,
                Amount = dto.Amount,
                CurrencyId = dto.CurrencyId,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim()
            };

            await ApplyContractorAsync(record, dto.Type, dto.ContractorClientId);
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

            var requireActiveCurrency = record.CurrencyId != dto.CurrencyId;
            await ValidateRecordFieldsAsync(dto.Amount, dto.CurrencyId, requireActiveCurrency);

            var requireActiveSubtype = record.SubtypeId != dto.SubtypeId;
            await ValidateSubtypeAsync(dto.Type, dto.SubtypeId, requireActiveSubtype);

            record.Type = dto.Type;
            record.SubtypeId = dto.SubtypeId;
            record.Date = dto.Date.Date;
            record.Amount = dto.Amount;
            record.CurrencyId = dto.CurrencyId;
            record.Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();
            await ApplyContractorAsync(record, dto.Type, dto.ContractorClientId);

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

        public async Task<IReadOnlyList<BuildingFinancialTotalDto>> GetBuildingTotalsAsync(
            DateTime startDate,
            DateTime endDate,
            IReadOnlyCollection<int>? propertyIds,
            FinancialRecordType? type)
        {
            var start = startDate.Date;
            var endExclusive = endDate.Date.AddDays(1);

            var query = _context.UnitFinancialRecords
                .AsNoTracking()
                .Where(record => record.Date >= start && record.Date < endExclusive);

            if (propertyIds is { Count: > 0 })
            {
                var ids = propertyIds.Distinct().ToList();
                query = query.Where(record => ids.Contains(record.Unit.PropertyId));
            }

            if (type.HasValue)
                query = query.Where(record => record.Type == type.Value);

            var grouped = await query
                .Select(record => new
                {
                    PropertyId = record.Unit.PropertyId,
                    Code = record.Currency.Code,
                    Symbol = record.Currency.Symbol,
                    Income = record.Type == FinancialRecordType.Income ? record.Amount : 0m,
                    Expense = record.Type == FinancialRecordType.Expense ? record.Amount : 0m
                })
                .GroupBy(row => new { row.PropertyId, row.Code, row.Symbol })
                .Select(group => new
                {
                    group.Key.PropertyId,
                    group.Key.Code,
                    group.Key.Symbol,
                    Income = group.Sum(row => row.Income),
                    Expense = group.Sum(row => row.Expense)
                })
                .ToListAsync();

            return grouped
                .Select(row => new BuildingFinancialTotalDto
                {
                    PropertyId = row.PropertyId,
                    CurrencyCode = row.Code,
                    CurrencySymbol = string.IsNullOrEmpty(row.Symbol) ? row.Code : row.Symbol,
                    Income = row.Income,
                    Expense = row.Expense
                })
                .ToList();
        }

        public async Task<IReadOnlyList<UnitIncomeTotalDto>> GetUnitIncomeTotalsAsync(
            DateTime startDate,
            DateTime endDate,
            IReadOnlyCollection<int>? propertyIds)
        {
            var start = startDate.Date;
            var endExclusive = endDate.Date.AddDays(1);

            var query = _context.UnitFinancialRecords
                .AsNoTracking()
                .Where(record => record.Type == FinancialRecordType.Income)
                .Where(record => record.Date >= start && record.Date < endExclusive);

            if (propertyIds is { Count: > 0 })
            {
                var ids = propertyIds.Distinct().ToList();
                query = query.Where(record => ids.Contains(record.Unit.PropertyId));
            }

            var grouped = await query
                .Select(record => new
                {
                    PropertyId = record.Unit.PropertyId,
                    record.UnitId,
                    UnitNumber = record.Unit.UnitNumber,
                    Code = record.Currency.Code,
                    Symbol = record.Currency.Symbol,
                    record.Amount
                })
                .GroupBy(row => new { row.PropertyId, row.UnitId, row.UnitNumber, row.Code, row.Symbol })
                .Select(group => new
                {
                    group.Key.PropertyId,
                    group.Key.UnitId,
                    group.Key.UnitNumber,
                    group.Key.Code,
                    group.Key.Symbol,
                    Amount = group.Sum(row => row.Amount),
                    IncomeCount = group.Count()
                })
                .ToListAsync();

            return grouped
                .Select(row => new UnitIncomeTotalDto
                {
                    PropertyId = row.PropertyId,
                    UnitId = row.UnitId,
                    UnitNumber = row.UnitNumber ?? "",
                    CurrencyCode = row.Code,
                    CurrencySymbol = string.IsNullOrEmpty(row.Symbol) ? row.Code : row.Symbol,
                    Amount = row.Amount,
                    IncomeCount = row.IncomeCount
                })
                .ToList();
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

        private async Task ValidateSubtypeAsync(FinancialRecordType type, int subtypeId, bool requireActive)
        {
            if (subtypeId <= 0)
                throw new InvalidOperationException("Subtype is required.");

            var subtypeQuery = _context.FinancialRecordSubtypes
                .AsNoTracking()
                .Where(s => s.Id == subtypeId && s.Type == type);

            if (requireActive)
                subtypeQuery = subtypeQuery.Where(s => s.IsActive);

            var exists = await subtypeQuery.AnyAsync();
            if (!exists)
                throw new InvalidOperationException("Subtype is not valid for this type.");
        }

        private static UnitFinancialRecordDto MapToDto(UnitFinancialRecord record) => new()
        {
            Id = record.Id,
            UnitId = record.UnitId,
            Type = record.Type,
            SubtypeId = record.SubtypeId,
            SubtypeName = record.Subtype.Name,
            SubtypeIsActive = record.Subtype.IsActive,
            Date = record.Date,
            Amount = record.Amount,
            CurrencyId = record.CurrencyId,
            CurrencyCode = record.Currency.Code,
            CurrencySymbol = record.Currency.Symbol ?? record.Currency.Code,
            ContractorClientId = record.ContractorClientId,
            ContractorName = record.ContractorClient == null
                ? null
                : record.ContractorClient.ClientType == ClientType.Company
                    ? record.ContractorClient.CompanyName
                    : $"{record.ContractorClient.FirstName} {record.ContractorClient.LastName}".Trim(),
            Comment = record.Comment
        };

        private async Task ApplyContractorAsync(UnitFinancialRecord record, FinancialRecordType type, int? contractorClientId)
        {
            if (type != FinancialRecordType.Expense || contractorClientId is null or <= 0)
            {
                record.ContractorClientId = null;
                return;
            }

            var exists = await _context.Clients.AnyAsync(c => c.Id == contractorClientId.Value);
            if (!exists)
                throw new InvalidOperationException("Contractor was not found.");

            record.ContractorClientId = contractorClientId;
            await _clientsService.EnsureRoleAsync(contractorClientId.Value, ClientRoleType.Contractor);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.FinancialRecordSubtypes;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services
{
    public class FinancialRecordSubtypesService : IFinancialRecordSubtypesService
    {
        private readonly PropertyManagerDbContext _context;

        public FinancialRecordSubtypesService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<FinancialRecordSubtypeDto>> GetAllAsync()
        {
            return await _context.FinancialRecordSubtypes
                .AsNoTracking()
                .OrderBy(s => s.Type)
                .ThenBy(s => s.SortOrder)
                .ThenBy(s => s.Name)
                .Select(s => new FinancialRecordSubtypeDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    SortOrder = s.SortOrder
                })
                .ToListAsync();
        }

        public async Task<FinancialRecordSubtypeDto?> GetByIdAsync(int id)
        {
            var subtype = await _context.FinancialRecordSubtypes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return subtype == null ? null : MapToDto(subtype);
        }

        public async Task<int> CreateAsync(CreateFinancialRecordSubtypeDto dto)
        {
            if (!Enum.IsDefined(dto.Type))
                throw new InvalidOperationException("Type is not valid.");

            var name = NormalizeName(dto.Name);
            await EnsureNameIsUniqueAsync(dto.Type, name, null);

            var maxOrder = await _context.FinancialRecordSubtypes
                .Where(s => s.Type == dto.Type)
                .Select(s => (int?)s.SortOrder)
                .MaxAsync() ?? 0;

            var subtype = new FinancialRecordSubtype
            {
                Type = dto.Type,
                Name = name,
                IsActive = true,
                SortOrder = maxOrder + 1
            };

            _context.FinancialRecordSubtypes.Add(subtype);
            await _context.SaveChangesAsync();

            return subtype.Id;
        }

        public async Task EditAsync(EditFinancialRecordSubtypeDto dto)
        {
            var subtype = await _context.FinancialRecordSubtypes
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (subtype == null)
                throw new InvalidOperationException("Subtype not found.");

            var name = NormalizeName(dto.Name);
            await EnsureNameIsUniqueAsync(subtype.Type, name, dto.Id);

            subtype.Name = name;
            subtype.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subtype = await _context.FinancialRecordSubtypes
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subtype == null)
                return;

            var inUse = await _context.UnitFinancialRecords
                .AnyAsync(record => record.SubtypeId == id);

            if (inUse)
                throw new InvalidOperationException("Subtype is used by a financial record.");

            _context.FinancialRecordSubtypes.Remove(subtype);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureNameIsUniqueAsync(FinancialRecordType type, string name, int? exceptId)
        {
            var normalized = name.ToLower();
            var query = _context.FinancialRecordSubtypes
                .Where(s => s.Type == type && s.Name.ToLower() == normalized);

            if (exceptId.HasValue)
                query = query.Where(s => s.Id != exceptId.Value);

            if (await query.AnyAsync())
                throw new InvalidOperationException("A subtype with this name already exists for this type.");
        }

        private static string NormalizeName(string? name)
        {
            var trimmed = name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmed))
                throw new InvalidOperationException("Name is required.");

            if (trimmed.Length > 100)
                throw new InvalidOperationException("Name must be 100 characters or fewer.");

            return trimmed;
        }

        private static FinancialRecordSubtypeDto MapToDto(FinancialRecordSubtype subtype) => new()
        {
            Id = subtype.Id,
            Type = subtype.Type,
            Name = subtype.Name,
            IsActive = subtype.IsActive,
            SortOrder = subtype.SortOrder
        };
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.PriceComponents;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Application.Services
{
    public class PriceComponentsService : IPriceComponentsService
    {
        private readonly PropertyManagerDbContext _context;

        public PriceComponentsService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PriceComponentDto>> GetAllAsync()
        {
            return await _context.PriceComponents
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new PriceComponentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IncludeInCommission = c.IncludeInCommission,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<PriceComponentDto?> GetByIdAsync(int id)
        {
            var component = await _context.PriceComponents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return component == null ? null : MapToDto(component);
        }

        public async Task<int> CreateAsync(CreatePriceComponentDto dto)
        {
            var name = NormalizeName(dto.Name);
            await EnsureNameIsUniqueAsync(name, null);

            var component = new PriceComponent
            {
                Name = name,
                IncludeInCommission = dto.IncludeInCommission,
                IsActive = true
            };

            _context.PriceComponents.Add(component);
            await _context.SaveChangesAsync();

            return component.Id;
        }

        public async Task EditAsync(EditPriceComponentDto dto)
        {
            var component = await _context.PriceComponents
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (component == null)
                throw new InvalidOperationException("Price component not found.");

            var name = NormalizeName(dto.Name);
            await EnsureNameIsUniqueAsync(name, dto.Id);

            component.Name = name;
            component.IncludeInCommission = dto.IncludeInCommission;
            component.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var component = await _context.PriceComponents
                .FirstOrDefaultAsync(c => c.Id == id);

            if (component == null)
                return;

            var inUse = await _context.UnitPriceComponents
                .AnyAsync(line => line.PriceComponentId == id);

            if (inUse)
                throw new InvalidOperationException("Price component is assigned to a unit.");

            _context.PriceComponents.Remove(component);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? exceptId)
        {
            var normalized = name.ToLower();
            var query = _context.PriceComponents.Where(c => c.Name.ToLower() == normalized);
            if (exceptId.HasValue)
                query = query.Where(c => c.Id != exceptId.Value);

            if (await query.AnyAsync())
                throw new InvalidOperationException("A price component with this name already exists.");
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

        private static PriceComponentDto MapToDto(PriceComponent component) => new()
        {
            Id = component.Id,
            Name = component.Name,
            IncludeInCommission = component.IncludeInCommission,
            IsActive = component.IsActive
        };
    }
}

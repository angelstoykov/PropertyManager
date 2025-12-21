using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Unit;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Application.Services
{
    public class UnitService : IUnitService
    {
        private readonly PropertyManagerDbContext _context;

        public UnitService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateUnitDto dto)
        {
            // ✅ Валидация – съществува ли Property
            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == dto.PropertyId);

            if (!propertyExists)
                throw new Exception("Property not found.");

            var unit = new Unit
            {
                PropertyId = dto.PropertyId,
                UnitNumber = dto.UnitNumber,
                Floor = dto.Floor,
                Area = dto.SizeSqM,
                Status = dto.Status
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return unit.Id;
        }

        public async Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query)
        {
            var unitsQuery = _context.Units
                .AsNoTracking()
                .Include(u => u.Property)
                .AsQueryable();

            // ✅ ФИЛТРИ
            if (query.PropertyId.HasValue)
                unitsQuery = unitsQuery
                    .Where(u => u.PropertyId == query.PropertyId.Value);

            if (query.Status.HasValue)
                unitsQuery = unitsQuery
                    .Where(u => u.Status == query.Status.Value);

            var totalCount = await unitsQuery.CountAsync();

            // ✅ PAGING
            var items = await unitsQuery
                .OrderBy(u => u.UnitNumber)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new UnitListItemDto
                {
                    Id = u.Id,
                    UnitNumber = u.UnitNumber,
                    PropertyName = u.Property.Name,
                    Floor = u.Floor,
                    Area = u.Area,
                    Status = u.Status
                })
                .ToListAsync();

            return new PagedResult<UnitListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task EditAsync(EditUnitDto dto)
        {
            var unit = await _context.Units.FindAsync(dto.Id);
            if (unit == null)
                throw new Exception("Unit not found.");

            // Update fields
            unit.Type = dto.Type;
            unit.UnitNumber = dto.UnitNumber;
            unit.Area = dto.Area;
            unit.PropertyId = dto.PropertyId;
            unit.Floor = dto.Floor;
            unit.Status = dto.Status;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
                throw new Exception("Unit not found.");

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
        }
    }
}

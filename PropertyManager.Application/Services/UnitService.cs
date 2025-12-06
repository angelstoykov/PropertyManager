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
    }
}

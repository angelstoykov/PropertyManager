using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;

namespace PropertyManager.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly PropertyManagerDbContext _context;

    public PropertyService(PropertyManagerDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(CreatePropertyDto dto)
    {
        // ✅ Валидация – съществува ли Property
        var propertyExists = await _context.Properties
            .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

        if (propertyExists)
            throw new InvalidOperationException("Property with this name already exists.");

        var property = new Property
        {
            Name = dto.Name,
            Address = dto.Address,
            NumberOfUnits = dto.NumberOfUnits,
            Owner = dto.Owner,
            Type = dto.Type,
            Status = dto.Status,
            Notes = dto.Notes,
            Description = dto.Description
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return property.Id;
    }

    public async Task<IEnumerable<PropertyListItemDto>> GetAllAsync()
    {
        return await _context.Properties
            .AsNoTracking()
            .Select(p => new PropertyListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address
            })
            .ToListAsync();
    }

    
}

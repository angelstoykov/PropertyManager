using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;

namespace PropertyManager.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly PropertyManagerDbContext _context;

    public PropertyService(PropertyManagerDbContext context)
    {
        _context = context;
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

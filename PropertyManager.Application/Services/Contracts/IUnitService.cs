using PropertyManager.Application.DTOs.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManager.Application.Services.Contracts
{
    public interface IUnitService
    {
        Task<int> CreateAsync(CreateUnitDto dto);

        Task<PagedResult<UnitListItemDto>> GetPagedAsync(UnitQueryDto query);

    }
}

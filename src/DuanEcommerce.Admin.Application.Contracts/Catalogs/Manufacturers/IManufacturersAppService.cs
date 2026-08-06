using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin.Manufacturers;

public interface IManufacturersAppService
    : ICrudAppService<ManufacturerDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateManufacturerDto, CreateUpdateManufacturerDto>
{
    Task<PagedResultDto<ManufacturerDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ManufacturerDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
}

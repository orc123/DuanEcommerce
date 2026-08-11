using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public.Manufacturers;

public interface IManufacturersAppService
    : IReadOnlyAppService<ManufacturerDto, Guid, PagedAndSortedResultRequestDto>
{
    Task<PagedResultDto<ManufacturerDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ManufacturerDto>> GetListAllAsync();
}

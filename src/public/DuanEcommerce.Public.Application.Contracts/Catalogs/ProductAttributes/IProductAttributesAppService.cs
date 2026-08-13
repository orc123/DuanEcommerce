using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public.ProductAttributes;

public interface IProductAttributesAppService 
    : IReadOnlyAppService<ProductAttributeDto, Guid, PagedAndSortedResultRequestDto>
{
    Task<PagedResultDto<ProductAttributeDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductAttributeDto>> GetListAllAsync();
}

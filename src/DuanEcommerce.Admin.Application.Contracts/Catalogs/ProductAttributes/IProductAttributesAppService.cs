using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin.ProductAttributes;

public interface IProductAttributesAppService 
    : ICrudAppService<ProductAttributeDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>
{
    Task<PagedResultDto<ProductAttributeDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductAttributeDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
}

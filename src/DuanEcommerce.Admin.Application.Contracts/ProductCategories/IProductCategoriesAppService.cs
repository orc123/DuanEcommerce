using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin.ProductCategories;

public interface IProductCategoriesAppService 
    : ICrudAppService<ProductCategoryDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>
{
    Task<PagedResultDto<ProductCategoryDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductCategoryDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
}

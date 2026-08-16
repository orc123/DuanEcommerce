using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public.ProductCategories;

public interface IProductCategoriesAppService 
    : IReadOnlyAppService<ProductCategoryDto, Guid, PagedAndSortedResultRequestDto>
{
    Task<PagedResult<ProductCategoryDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductCategoryDto>> GetListAllAsync();
    Task<ProductCategoryDto?> GetByCodeAsync(string code);
    Task<ProductCategoryDto?> GetBySlugAsync(string slug);
}

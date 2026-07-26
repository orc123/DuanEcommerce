using DuanEcommerce.ProductCategories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Admin.ProductCategories;

public class ProductCategoriesAppService : CrudAppService
    <ProductCategory, ProductCategoryDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
{
    public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
    {
    }

    public override async Task<PagedResultDto<ProductCategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await Repository.GetQueryableAsync();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var entities = await AsyncExecuter.ToListAsync(query);

        var entityDtos = totalCount == 0 ? [] : ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(entities);

        return new PagedResultDto<ProductCategoryDto>(totalCount, entityDtos);
    }
}

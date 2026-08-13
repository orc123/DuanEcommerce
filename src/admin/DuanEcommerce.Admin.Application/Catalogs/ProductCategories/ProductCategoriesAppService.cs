using DuanEcommerce.Admin.Permissions;
using DuanEcommerce.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Admin.ProductCategories;

[Authorize(DuanEcommercePermissions.ProductCategory.Default)]
public class ProductCategoriesAppService : CrudAppService
    <ProductCategory, ProductCategoryDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
{
    public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
    {
        GetPolicyName = DuanEcommercePermissions.ProductCategory.Default;
        GetListPolicyName = DuanEcommercePermissions.ProductCategory.Default;
        CreatePolicyName = DuanEcommercePermissions.ProductCategory.Create;
        UpdatePolicyName = DuanEcommercePermissions.ProductCategory.Update;
        DeletePolicyName = DuanEcommercePermissions.ProductCategory.Delete;
    }

    [Authorize(DuanEcommercePermissions.ProductCategory.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    [Authorize(DuanEcommercePermissions.ProductCategory.Default)]
    public async Task<List<ProductCategoryDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(data);
    }

    [Authorize(DuanEcommercePermissions.ProductCategory.Default)]
    public async Task<PagedResultDto<ProductCategoryDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductCategoryDto>(totalCount, ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(data));
    }
}

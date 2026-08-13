using DuanEcommerce.Admin.Permissions;
using DuanEcommerce.ProductAttributes;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Admin.ProductAttributes;

[Authorize(DuanEcommercePermissions.Attribute.Default)]
public class ProductAttributesAppService : CrudAppService
    <ProductAttribute, ProductAttributeDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>, IProductAttributesAppService
{
    public ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
    {
        GetPolicyName = DuanEcommercePermissions.Attribute.Default;
        GetListPolicyName = DuanEcommercePermissions.Attribute.Default;
        CreatePolicyName = DuanEcommercePermissions.Attribute.Create;
        UpdatePolicyName = DuanEcommercePermissions.Attribute.Update;
        DeletePolicyName = DuanEcommercePermissions.Attribute.Delete;
    }
    [Authorize(DuanEcommercePermissions.Attribute.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    [Authorize(DuanEcommercePermissions.Attribute.Default)]
    public async Task<List<ProductAttributeDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeDto>>(data);
    }

    [Authorize(DuanEcommercePermissions.Attribute.Default)]
    public async Task<PagedResultDto<ProductAttributeDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Label.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductAttributeDto>(totalCount, ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeDto>>(data));
    }
}

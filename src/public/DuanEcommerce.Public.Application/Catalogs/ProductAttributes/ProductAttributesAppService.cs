using DuanEcommerce.ProductAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Public.ProductAttributes;

public class ProductAttributesAppService : ReadOnlyAppService
    <ProductAttribute, ProductAttributeDto, Guid, PagedAndSortedResultRequestDto> , IProductAttributesAppService
{
    public ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
    {
    }

    public async Task<List<ProductAttributeDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeDto>>(data);
    }

    public async Task<PagedResultDto<ProductAttributeDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Label.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductAttributeDto>(totalCount, ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeDto>>(data));
    }
}

using DuanEcommerce.Manufacturers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Public.Manufacturers;

public class ManufacturersAppService : ReadOnlyAppService
    <Manufacturer, ManufacturerDto, Guid, PagedAndSortedResultRequestDto>, IManufacturersAppService
{
    public ManufacturersAppService(IRepository<Manufacturer, Guid> repository) : base(repository)
    {
    }

    public async Task<List<ManufacturerDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(data);
    }


    public async Task<PagedResult<ManufacturerDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter
            .ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize));

        return new PagedResult<ManufacturerDto>
            (ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(data), totalCount, input.CurrentPage, input.PageSize);
    }
}

using DuanEcommerce.Admin.Permissions;
using DuanEcommerce.Manufacturers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Admin.Manufacturers;

[Authorize(DuanEcommercePermissions.Manufacturer.Default)]
public class ManufacturersAppService : CrudAppService
    <Manufacturer, ManufacturerDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateManufacturerDto, CreateUpdateManufacturerDto>, IManufacturersAppService
{
    public ManufacturersAppService(IRepository<Manufacturer, Guid> repository) : base(repository)
    {
        GetPolicyName = DuanEcommercePermissions.Manufacturer.Default;
        GetListPolicyName = DuanEcommercePermissions.Manufacturer.Default;
        CreatePolicyName = DuanEcommercePermissions.Manufacturer.Create;
        UpdatePolicyName = DuanEcommercePermissions.Manufacturer.Update;
        DeletePolicyName = DuanEcommercePermissions.Manufacturer.Delete;
    }


    [Authorize(DuanEcommercePermissions.Manufacturer.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    [Authorize(DuanEcommercePermissions.Manufacturer.Default)]
    public async Task<List<ManufacturerDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(data);
    }


    [Authorize(DuanEcommercePermissions.Manufacturer.Default)]
    public async Task<PagedResultDto<ManufacturerDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ManufacturerDto>(totalCount, ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(data));
    }
}

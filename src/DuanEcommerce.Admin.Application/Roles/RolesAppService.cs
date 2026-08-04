using DuanEcommerce.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace DuanEcommerce.Admin.Roles;

public class RolesAppService(IRepository<IdentityRole, Guid> repository) : CrudAppService
    <IdentityRole, RoleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>(repository), IRolesAppService
{
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    public async Task<List<RoleDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        var data = await AsyncExecuter.ToListAsync(query.Select(x => new RoleDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.GetProperty<string>(RoleConsts.DescriptionFieldName)
        }));

        return data;
    }

    public async Task<PagedResultDto<RoleDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword!)).AsQueryable();

        var totalCount = await AsyncExecuter.CountAsync(query);

        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount).Select(x => new RoleDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.GetProperty<string>(RoleConsts.DescriptionFieldName)
        }));

        return new PagedResultDto<RoleDto>(totalCount, data);
    }

    public async override Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
    {
        var query = await Repository.GetQueryableAsync();
        var isNameExisted = await AsyncExecuter.AnyAsync(query, x => x.Name == input.Name);
        if (isNameExisted)
        {
            throw new BusinessException(DuanEcommerceDomainErrorCodes.RoleNameAlreadyExists)
                .WithData("Name", input.Name);
        }

        // Sử dụng GuidGenerator của ABP thay vì Guid.NewGuid()
        var role = new IdentityRole(GuidGenerator.Create(), input.Name);
        role.SetProperty(RoleConsts.DescriptionFieldName, input.Description);

        var data = await Repository.InsertAsync(role);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new RoleDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = input.Description
        };
    }

    public async override Task<RoleDto> UpdateAsync(Guid id, CreateUpdateRoleDto input)
    {
        var role = await Repository.GetAsync(id);
        if (role == null)
        {
            throw new EntityNotFoundException(typeof(IdentityRole), id);
        }
        var query = await Repository.GetQueryableAsync();
        var isNameExisted = await AsyncExecuter.AnyAsync(query, x => x.Name == input.Name && x.Id != id);
        if (isNameExisted)
        {
            throw new BusinessException(DuanEcommerceDomainErrorCodes.RoleNameAlreadyExists)
                .WithData("Name", input.Name);
        }
        role.SetProperty(RoleConsts.DescriptionFieldName, input.Description);
        var data = await Repository.UpdateAsync(role);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new RoleDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = input.Description
        };
    }
}
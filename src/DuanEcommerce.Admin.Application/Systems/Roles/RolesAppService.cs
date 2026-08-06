using DuanEcommerce.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Localization;
using Volo.Abp.SimpleStateChecking;

namespace DuanEcommerce.Admin.Roles;

public class RolesAppService : CrudAppService
    <IdentityRole, RoleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>, IRolesAppService
{
    protected PermissionManagementOptions Options { get; }
    protected IPermissionManager PermissionManager { get; }
    protected IPermissionChecker PermissionChecker { get; }
    protected IResourcePermissionManager ResourcePermissionManager { get; }
    protected IResourcePermissionGrantRepository ResourcePermissionGrantRepository { get; }
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
    protected ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

    public RolesAppService(
        IRepository<IdentityRole, Guid> repository,
        IPermissionManager permissionManager,
        IPermissionChecker permissionChecker,
        IPermissionDefinitionManager permissionDefinitionManager,
        IResourcePermissionManager resourcePermissionManager,
        IResourcePermissionGrantRepository resourcePermissionGrantRepository,
        IOptions<PermissionManagementOptions> options,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager)
        : base(repository)
    {
        LocalizationResource = typeof(AbpPermissionManagementResource);
        ObjectMapperContext = typeof(AbpPermissionManagementApplicationModule);

        Options = options.Value;
        PermissionManager = permissionManager;
        PermissionChecker = permissionChecker;
        ResourcePermissionManager = resourcePermissionManager;
        ResourcePermissionGrantRepository = resourcePermissionGrantRepository;
        PermissionDefinitionManager = permissionDefinitionManager;
        SimpleStateCheckerManager = simpleStateCheckerManager;
    }
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

    public async override Task<RoleDto> GetAsync(Guid id)
    {
        var role = await Repository.GetAsync(id);
        if (role == null)
        {
            throw new EntityNotFoundException(typeof(IdentityRole), id);
        }
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.GetProperty<string>(RoleConsts.DescriptionFieldName)
        };
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

    public async Task<GetPermissionListResultDto> GetPermissionsAsync(string providerName, string providerKey)
    {
        //await CheckProviderPolicy(providerName);

        var result = new GetPermissionListResultDto
        {
            EntityDisplayName = providerKey,
            Groups = new List<PermissionGroupDto>()
        };

        foreach (var group in (await PermissionDefinitionManager.GetGroupsAsync()).Where(x => x.Name.StartsWith("AbpIdentity") || x.Name.StartsWith("DuanEcomAdmin")))
        {
            var groupDto = CreatePermissionGroupDto(group);

            var neededCheckPermissions = new List<PermissionDefinition>();

            foreach (var permission in group.GetPermissionsWithChildren()
                                            .Where(x => x.IsEnabled)
                                            .Where(x => !x.Providers.Any() || x.Providers.Contains(providerName)))
            {
                if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
                {
                    neededCheckPermissions.Add(permission);
                }
            }

            if (!neededCheckPermissions.Any())
            {
                continue;
            }

            var grantInfoDtos = neededCheckPermissions
                .Select(CreatePermissionGrantInfoDto)
                .ToList();

            var multipleGrantInfo = await PermissionManager.GetAsync(neededCheckPermissions.Select(x => x.Name).ToArray(), providerName, providerKey);

            foreach (var grantInfo in multipleGrantInfo.Result)
            {
                var grantInfoDto = grantInfoDtos.First(x => x.Name == grantInfo.Name);

                grantInfoDto.IsGranted = grantInfo.IsGranted;

                foreach (var provider in grantInfo.Providers)
                {
                    grantInfoDto.GrantedProviders.Add(new ProviderInfoDto
                    {
                        ProviderName = provider.Name,
                        ProviderKey = provider.Key,
                    });
                }

                groupDto.Permissions.Add(grantInfoDto);
            }

            if (groupDto.Permissions.Any())
            {
                result.Groups.Add(groupDto);
            }
        }

        return result;
    }

    private PermissionGrantInfoDto CreatePermissionGrantInfoDto(PermissionDefinition permission)
    {
        return new PermissionGrantInfoDto
        {
            Name = permission.Name,
            DisplayName = permission.DisplayName?.Localize(StringLocalizerFactory),
            ParentName = permission.Parent?.Name,
            AllowedProviders = permission.Providers,
            GrantedProviders = new List<ProviderInfoDto>()
        };
    }

    private PermissionGroupDto CreatePermissionGroupDto(PermissionGroupDefinition group)
    {
        return new PermissionGroupDto
        {
            Name = group.Name,
            DisplayName = group.DisplayName?.Localize(StringLocalizerFactory),
            Permissions = new List<PermissionGrantInfoDto>(),
        };
    }

    [Authorize(IdentityPermissions.Roles.Update)]

    public virtual async Task UpdatePermissionsAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {
        // await CheckProviderPolicy(providerName);

        foreach (var permissionDto in input.Permissions)
        {
            await PermissionManager.SetAsync(permissionDto.Name, providerName, providerKey, permissionDto.IsGranted);
        }
    }

    protected virtual async Task CheckProviderPolicy(string providerName)
    {
        var policyName = Options.ProviderPolicies.GetOrDefault(providerName);
        if (policyName.IsNullOrEmpty())
        {
            throw new AbpException($"No policy defined to get/set permissions for the provider '{providerName}'. Use {nameof(PermissionManagementOptions)} to map the policy.");
        }

        await AuthorizationService.CheckAsync(policyName);
    }

}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.PermissionManagement;

namespace DuanEcommerce.Admin.Roles;

public interface IRolesAppService 
    : ICrudAppService<RoleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>
{
    Task<PagedResultDto<RoleDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<RoleDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
    Task<GetPermissionListResultDto> GetPermissionsAsync(string providerName, string providerKey);
    Task UpdatePermissionsAsync(string providerName, string providerKey, UpdatePermissionsDto input);
}

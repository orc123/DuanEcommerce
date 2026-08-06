using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin.Users;

public interface IUsersAppService
     : ICrudAppService<UserDto, Guid, PagedAndSortedResultRequestDto, CreateUserDto, UpdateUserDto>
{
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);

    Task<PagedResultDto<UserDto>> GetListWithFilterAsync(BaseListFilterDto input);

    Task<List<UserDto>> GetListAllAsync(string filterKeyword);
}

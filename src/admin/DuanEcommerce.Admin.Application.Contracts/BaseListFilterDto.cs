using Volo.Abp.Application.Dtos;

namespace DuanEcommerce.Admin;

public class BaseListFilterDto : PagedResultRequestDto
{
    public string? Keyword { get; set; } = null;
}

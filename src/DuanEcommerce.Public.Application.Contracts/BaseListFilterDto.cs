using Volo.Abp.Application.Dtos;

namespace DuanEcommerce.Public;

public class BaseListFilterDto : PagedResultRequestDto
{
    public string? Keyword { get; set; } = null;
}

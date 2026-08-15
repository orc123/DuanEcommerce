using Volo.Abp.Application.Dtos;

namespace DuanEcommerce.Public;

public class BaseListFilterDto : PagedResultRequestBase
{
    public string? Keyword { get; set; } = null;
}

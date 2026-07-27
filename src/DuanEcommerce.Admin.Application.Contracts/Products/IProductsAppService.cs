using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin.Products;

public interface IProductsAppService : ICrudAppService
    <ProductDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateProductDto,
    CreateUpdateProductDto
    >
{
    Task<PagedResultDto<ProductDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids); 
}

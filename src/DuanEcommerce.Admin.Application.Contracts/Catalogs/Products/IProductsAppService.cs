using DuanEcommerce.Admin.Products.Attributes;
using System;
using System.Collections.Generic;
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
    Task<PagedResultDto<ProductDto>> GetListFilterAsync(ProductListFilterDto input);
    Task<List<ProductDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
    Task<string?> GetThumbnailImageAsync(string fileName);
    Task<string> GetSuggestNewCodeAsync();
    Task<ProductAttributeValueDto> AddAttributeAsync(AddUpdateProductAttributeDto input);
    Task<ProductAttributeValueDto> UpdateAttributeAsync(Guid id, AddUpdateProductAttributeDto input);
    Task RemoveProductAttributeAsync(Guid attributeId, Guid id);
    Task<List<ProductAttributeValueDto>> GetProductAttributeAllAsync(Guid productId);
    Task<PagedResultDto<ProductAttributeValueDto>> GetProductAttributesAsync(ProductAttributeListFilterDto input);
}

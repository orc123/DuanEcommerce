using DuanEcommerce.Public.Products.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public.Products;

public interface IProductsAppService : IReadOnlyAppService
    <ProductDto,
    Guid,
    PagedAndSortedResultRequestDto
    >
{
    Task<PagedResult<ProductDto>> GetListFilterAsync(ProductListFilterDto input);
    Task<List<ProductDto>> GetListAllAsync();
    Task<List<ProductAttributeValueDto>> GetProductAttributeAllAsync(Guid productId);
    Task<PagedResult<ProductAttributeValueDto>> GetProductAttributesAsync(ProductAttributeListFilterDto input);
    Task<List<ProductDto>> GetListTopSellerAsync(int numberOfRecords);
    Task<ProductDto?> GetBySlugAsync(string slug);
    Task<string> GetThumbnailImageAsync(string fileName);
}

using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Admin.Products;

public class ProductsAppService(
        IRepository<Product, Guid> repository, 
        ProductManager productManager, 
        IRepository<ProductCategory> productCategoryRepository,
        IBlobContainer<ProductThumbnailPictureContainer> fileContainer
    ) : CrudAppService
    <Product,
    ProductDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateProductDto,
    CreateUpdateProductDto
    >(repository), IProductsAppService
{
    private readonly ProductManager _productManager = productManager;
    private readonly IRepository<ProductCategory> _productCategoryRepository = productCategoryRepository;
    private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer = fileContainer;

    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
    {
        var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug,
            input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive,
            input.CategoryId, input.SeoMetaDescription, input.Description, input.SellPrice);

        if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
        {
            await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
            product.ThumbnailPicture = input.ThumbnailPictureName;
        }

        var result = await Repository.InsertAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(result);
    }

    public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
    {
        var product = await Repository.GetAsync(id) ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductIsNotExists);

        if (await Repository.AnyAsync(x => x.Id != id && x.Name == input.Name))
        {
            throw new UserFriendlyException("Tên sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductNameAlreadyExists);
        }
        if (await Repository.AnyAsync(x => x.Id != id && x.Code == input.Code))
        {
            throw new UserFriendlyException("Mã sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductCodeAlreadyExists);
        }
        if (await Repository.AnyAsync(x => x.Id != id && x.SKU == input.SKU))
        {
            throw new UserFriendlyException("Mã SKU sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductSKUAlreadyExists);
        }

        product.ManufacturerId = input.ManufacturerId;
        product.Name = input.Name;
        product.Code = input.Code;
        product.Slug = input.Slug;
        product.ProductType = input.ProductType;
        product.SKU = input.SKU;
        product.SortOrder = input.SortOrder;
        product.Visibility = input.Visibility;
        product.IsActive = input.IsActive;

        if (product.CategoryId != input.CategoryId)
        {
            product.CategoryId = input.CategoryId;
            var category = await _productCategoryRepository.GetAsync(x => x.Id == input.CategoryId);
            product.CategoryName = category.Name;
            product.CategorySlug = category.Slug;
        }
        product.SeoMetaDescription = input.SeoMetaDescription;
        product.Description = input.Description;
        if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
        {
            await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
            product.ThumbnailPicture = input.ThumbnailPictureName;
        }
        product.SellPrice = input.SellPrice;
        await Repository.UpdateAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<List<ProductDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Product>, List<ProductDto>>(data);
    }

    public async Task<PagedResultDto<ProductDto>> GetListFilterAsync(ProductListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
        query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId ==  input.CategoryId.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductDto>>(data));
    }

    private async Task SaveThumbnailImageAsync(string fileName, string base64)
    {
        Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
        base64 = regex.Replace(base64, string.Empty);
        byte[] bytes = Convert.FromBase64String(base64);
        await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
    }
}

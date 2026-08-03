using DuanEcommerce.Admin.Products.Attributes;
using DuanEcommerce.ProductAttributes;
using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using Microsoft.AspNetCore.Authorization;
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

[Authorize]
public class ProductsAppService(
        IRepository<Product, Guid> repository,
        ProductManager productManager,
        IRepository<ProductCategory> productCategoryRepository,
        IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
        ProductCodeGenerator productCodeGenerator,
        IRepository<ProductAttribute, Guid> productAttributeRepository,
        IRepository<ProductAttributeDateTime, Guid> productAttributeDateTimeRepository,
        IRepository<ProductAttributeInt, Guid> productAttributeIntRepository,
        IRepository<ProductAttributeDecimal, Guid> productAttributeDecimalRepository,
        IRepository<ProductAttributeText, Guid> productAttributeTextRepository,
        IRepository<ProductAttributeVarchar, Guid> productAttributeVarcharRepository
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
    private readonly ProductCodeGenerator _productCodeGenerator = productCodeGenerator;
    private readonly IRepository<ProductAttribute, Guid> _productAttributeRepository = productAttributeRepository;
    private readonly IRepository<ProductAttributeDateTime, Guid> _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
    private readonly IRepository<ProductAttributeInt, Guid> _productAttributeIntRepository = productAttributeIntRepository;
    private readonly IRepository<ProductAttributeDecimal, Guid> _productAttributeDecimalRepository = productAttributeDecimalRepository;
    private readonly IRepository<ProductAttributeText, Guid> _productAttributeTextRepository = productAttributeTextRepository;
    private readonly IRepository<ProductAttributeVarchar, Guid> _productAttributeVarcharRepository = productAttributeVarcharRepository;

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
        query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductDto>>(data));
    }

    public async Task<string?> GetThumbnailImageAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        var thumbnailPictureContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

        if (thumbnailPictureContent == null)
        {
            return null;
        }
        return Convert.ToBase64String(thumbnailPictureContent);
    }

    public async Task<string> GetSuggestNewCodeAsync()
    {
        return await _productCodeGenerator.GenerateAsync();
    }

    private async Task SaveThumbnailImageAsync(string fileName, string base64)
    {
        Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
        base64 = regex.Replace(base64, string.Empty);
        byte[] bytes = Convert.FromBase64String(base64);
        await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
    }

    public async Task<ProductAttributeValueDto> AddAttributeAsync(AddUpdateProductAttributeDto input)
    {
        var product = await Repository.GetAsync(input.ProductId)
             ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductIsNotExists); ;
        var attribute = await _productAttributeRepository.GetAsync(input.AttributeId)
            ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
        var newAttributeId = Guid.NewGuid();
        switch (attribute.DataType)
        {
            case AttributeType.Int:
                if (!input.IntValue.HasValue)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var intAttribute = new ProductAttributeInt(newAttributeId, input.ProductId, input.AttributeId, input.IntValue.Value);
                await _productAttributeIntRepository.InsertAsync(intAttribute);
                break;
            case AttributeType.Varchar:
                if (string.IsNullOrEmpty(input.VarcharValue))
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var varcharAttribute = new ProductAttributeVarchar(newAttributeId, input.ProductId, input.AttributeId, input.VarcharValue);
                await _productAttributeVarcharRepository.InsertAsync(varcharAttribute);
                break;
            case AttributeType.Text:
                if (string.IsNullOrEmpty(input.TextValue))
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var textAttribute = new ProductAttributeText(newAttributeId, input.ProductId, input.AttributeId, input.TextValue);
                await _productAttributeTextRepository.InsertAsync(textAttribute);
                break;
            case AttributeType.Decimal:
                if (input.DecimalValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var decimalAttribute = new ProductAttributeDecimal(newAttributeId, input.ProductId, input.AttributeId, input.DecimalValue.Value);
                await _productAttributeDecimalRepository.InsertAsync(decimalAttribute);
                break;
            case AttributeType.Date:
                if (input.DateTimeValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var datetimeAttribute = new ProductAttributeDateTime(newAttributeId, input.ProductId, input.AttributeId, input.DateTimeValue.Value);
                await _productAttributeDateTimeRepository.InsertAsync(datetimeAttribute);
                break;
            default:
                break;
        }

        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new ProductAttributeValueDto
        {
            Id = newAttributeId,
            ProductId = input.ProductId,
            AttributeId = input.AttributeId,
            IntValue = input.IntValue,
            VarcharValue = input.VarcharValue,
            TextValue = input.TextValue,
            DecimalValue = input.DecimalValue,
            DateTimeValue = input.DateTimeValue
        };
    }

    public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
    {
        var attribute = await _productAttributeRepository.GetAsync(x => x.Id == id)
           ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

        switch (attribute.DataType)
        {
            case AttributeType.Int:
                var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                await _productAttributeIntRepository.DeleteAsync(productAttributeInt);
                break;
            case AttributeType.Varchar:
                var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                await _productAttributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                break;
            case AttributeType.Text:
                var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                await _productAttributeTextRepository.DeleteAsync(productAttributeText);
                break;
            case AttributeType.Decimal:
                var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                await _productAttributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                break;
            case AttributeType.Date:
                var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                await _productAttributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                break;
            default:
                break;
        }
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    public async Task<List<ProductAttributeValueDto>> GetProductAttributeAllAsync(Guid productId)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

        var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
        var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
        var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();
        var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
        var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();

        var query = from a in attributeQuery
                    join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTabke
                    from adate in aDateTimeTabke.DefaultIfEmpty()
                    join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                    from adecimal in aDecimalTable.DefaultIfEmpty()
                    join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                    from aint in aIntTable.DefaultIfEmpty()
                    join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                    from aVarchar in aVarcharTable.DefaultIfEmpty()
                    join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                    from aText in aTextTable.DefaultIfEmpty()
                    where (adate != null || adate.ProductId == productId)
                        && (adecimal != null || adecimal.ProductId == productId)
                        && (aint != null || aint.ProductId == productId)
                        && (aVarchar != null || aVarchar.ProductId == productId)
                        && (aText != null || aText.ProductId == productId)
                    select new ProductAttributeValueDto()
                    {
                        Label = a.Label,
                        AttributeId = a.Id,
                        DataType = a.DataType,
                        Code = a.Code,
                        ProductId = productId,
                        DateTimeValue = adate.Value,
                        DecimalValue = adecimal.Value,
                        IntValue = aint.Value,
                        TextValue = aText.Value,
                        VarcharValue = aVarchar.Value,
                        DecimalId = adecimal.Id,
                        IntId = aint.Id,
                        TextId = aText.Id,
                        VarcharId = aVarchar.Id,
                    };
        return await AsyncExecuter.ToListAsync(query);
    }

    public async Task<PagedResultDto<ProductAttributeValueDto>> GetProductAttributesAsync(ProductAttributeListFilterDto input)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

        var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
        var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
        var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
        var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
        var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

        var query = from a in attributeQuery
                    join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTabke
                    from adate in aDateTimeTabke.DefaultIfEmpty()
                    join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                    from adecimal in aDecimalTable.DefaultIfEmpty()
                    join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                    from aint in aIntTable.DefaultIfEmpty()
                    join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                    from aVarchar in aVarcharTable.DefaultIfEmpty()
                    join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                    from aText in aTextTable.DefaultIfEmpty()
                    where (adate != null || adate.ProductId == input.ProductId)
                    && (adecimal != null || adecimal.ProductId == input.ProductId)
                     && (aint != null || aint.ProductId == input.ProductId)
                      && (aVarchar != null || aVarchar.ProductId == input.ProductId)
                       && (aText != null || aText.ProductId == input.ProductId)
                    select new ProductAttributeValueDto()
                    {
                        Label = a.Label,
                        AttributeId = a.Id,
                        DataType = a.DataType,
                        Code = a.Code,
                        ProductId = input.ProductId,
                        DateTimeValue = adate.Value,
                        DecimalValue = adecimal.Value,
                        IntValue = aint.Value,
                        TextValue = aText.Value,
                        VarcharValue = aVarchar.Value,
                        DecimalId = adecimal.Id,
                        IntId = aint.Id,
                        TextId = aText.Id,
                        VarcharId = aVarchar.Id,
                    };
        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.Label)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            );
        return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
    }

    public async Task<ProductAttributeValueDto> UpdateAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
    {
        var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductIsNotExists);

        var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId) ??
            throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

        switch (attribute.DataType)
        {
            case AttributeType.Date:
                if (input.DateTimeValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                productAttributeDateTime.Value = input.DateTimeValue.Value;
                await _productAttributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                break;
            case AttributeType.Int:
                if (input.IntValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                productAttributeInt.Value = input.IntValue.Value;
                await _productAttributeIntRepository.UpdateAsync(productAttributeInt);
                break;
            case AttributeType.Decimal:
                if (input.DecimalValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                if (productAttributeDecimal == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }
                productAttributeDecimal.Value = input.DecimalValue.Value;
                await _productAttributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                break;
            case AttributeType.Varchar:
                if (input.VarcharValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                productAttributeVarchar.Value = input.VarcharValue;
                await _productAttributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                break;
            case AttributeType.Text:
                if (input.TextValue == null)
                {
                    throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }
                var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id)
                    ?? throw new BusinessException(DuanEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                productAttributeText.Value = input.TextValue;
                await _productAttributeTextRepository.UpdateAsync(productAttributeText);
                break;
        }
        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new ProductAttributeValueDto()
        {
            AttributeId = input.AttributeId,
            Code = attribute.Code,
            DataType = attribute.DataType,
            DateTimeValue = input.DateTimeValue,
            DecimalValue = input.DecimalValue,
            Id = id,
            IntValue = input.IntValue,
            Label = attribute.Label,
            ProductId = input.ProductId,
            TextValue = input.TextValue
        };
    }
}


using DuanEcommerce.ProductAttributes;
using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using DuanEcommerce.Public.Products.Attributes;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DuanEcommerce.Public.Products;

public class ProductsAppService : ReadOnlyAppService
    <Product,
    ProductDto,
    Guid,
    PagedAndSortedResultRequestDto
    >, IProductsAppService
{
    private readonly ProductManager _productManager;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
    private readonly ProductCodeGenerator _productCodeGenerator;
    private readonly IRepository<ProductAttribute, Guid> _productAttributeRepository;
    private readonly IRepository<ProductAttributeDateTime, Guid> _productAttributeDateTimeRepository;
    private readonly IRepository<ProductAttributeInt, Guid> _productAttributeIntRepository;
    private readonly IRepository<ProductAttributeDecimal, Guid> _productAttributeDecimalRepository;
    private readonly IRepository<ProductAttributeText, Guid> _productAttributeTextRepository;
    private readonly IRepository<ProductAttributeVarchar, Guid> _productAttributeVarcharRepository;
    public ProductsAppService(
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
        IRepository<ProductAttributeVarchar, Guid> productAttributeVarcharRepository) : base(repository)
    {
        _productManager = productManager;
        _productCategoryRepository = productCategoryRepository;
        _fileContainer = fileContainer;
        _productCodeGenerator = productCodeGenerator;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
        _productAttributeIntRepository = productAttributeIntRepository;
        _productAttributeDecimalRepository = productAttributeDecimalRepository;
        _productAttributeTextRepository = productAttributeTextRepository;
        _productAttributeVarcharRepository = productAttributeVarcharRepository;

    }

    public async Task<List<ProductDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Product>, List<ProductDto>>(data);
    }

    public async Task<PagedResult<ProductDto>> GetListFilterAsync(ProductListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
        query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize));

        return new PagedResult<ProductDto>(ObjectMapper.Map<List<Product>, List<ProductDto>>(data), totalCount, input.CurrentPage, input.PageSize);
    }

    public async Task<List<ProductDto>> GetListTopSellerAsync(int numberOfRecords)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true)
            .OrderByDescending(x => x.CreationTime)
            .Take(numberOfRecords);

        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Product>, List<ProductDto>>(data);
    }

    public async Task<List<ProductAttributeValueDto>> GetProductAttributeAllAsync(Guid productId)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();
        var intValues = from v in (await _productAttributeIntRepository.GetQueryableAsync()).Where(x => x.ProductId == productId)
                        join a in attributeQuery on v.AttributeId equals a.Id
                        select new ProductAttributeValueDto
                        {
                            AttributeId = a.Id,
                            Code = a.Code,
                            Label = a.Label,
                            DataType = a.DataType,
                            ProductId = productId,
                            IntValue = v.Value,
                            IntId = v.Id,
                            VarcharValue = null,
                            VarcharId = null,
                            TextValue = null,
                            TextId = null,
                            DecimalValue = null,
                            DecimalId = null,
                            DateTimeValue = null,
                            DateTimeId = null
                        };
        var varcharValues = from v in (await _productAttributeVarcharRepository.GetQueryableAsync()).Where(x => x.ProductId == productId)
                            join a in attributeQuery on v.AttributeId equals a.Id
                            select new ProductAttributeValueDto
                            {
                                AttributeId = a.Id,
                                Code = a.Code,
                                Label = a.Label,
                                DataType = a.DataType,
                                ProductId = productId,
                                IntValue = null,
                                IntId = null,
                                VarcharValue = v.Value,
                                VarcharId = v.Id,
                                TextValue = null,
                                TextId = null,
                                DecimalValue = null,
                                DecimalId = null,
                                DateTimeValue = null,
                                DateTimeId = null
                            };
        var textValues = from v in (await _productAttributeTextRepository.GetQueryableAsync()).Where(x => x.ProductId == productId)
                         join a in attributeQuery on v.AttributeId equals a.Id
                         select new ProductAttributeValueDto
                         {
                             AttributeId = a.Id,
                             Code = a.Code,
                             Label = a.Label,
                             DataType = a.DataType,
                             ProductId = productId,
                             IntValue = null,
                             IntId = null,
                             VarcharValue = null,
                             VarcharId = null,
                             TextValue = v.Value,
                             TextId = v.Id,
                             DecimalValue = null,
                             DecimalId = null,
                             DateTimeValue = null,
                             DateTimeId = null
                         };
        var decimalValues = from v in (await _productAttributeDecimalRepository.GetQueryableAsync()).Where(x => x.ProductId == productId)
                            join a in attributeQuery on v.AttributeId equals a.Id
                            select new ProductAttributeValueDto
                            {
                                AttributeId = a.Id,
                                Code = a.Code,
                                Label = a.Label,
                                DataType = a.DataType,
                                ProductId = productId,
                                IntValue = null,
                                IntId = null,
                                VarcharValue = null,
                                VarcharId = null,
                                TextValue = null,
                                TextId = null,
                                DecimalValue = v.Value,
                                DecimalId = v.Id,
                                DateTimeValue = null,
                                DateTimeId = null
                            };
        var dateTimeValues = from v in (await _productAttributeDateTimeRepository.GetQueryableAsync()).Where(x => x.ProductId == productId)
                             join a in attributeQuery on v.AttributeId equals a.Id
                             select new ProductAttributeValueDto
                             {
                                 AttributeId = a.Id,
                                 Code = a.Code,
                                 Label = a.Label,
                                 DataType = a.DataType,
                                 ProductId = productId,
                                 IntValue = null,
                                 IntId = null,
                                 VarcharValue = null,
                                 VarcharId = null,
                                 TextValue = null,
                                 TextId = null,
                                 DecimalValue = null,
                                 DecimalId = null,
                                 DateTimeValue = v.Value,
                                 DateTimeId = v.Id
                             };
        // Gộp kết quả của 5 bảng lại bằng Concat (UNION ALL trong SQL)
        var combinedQuery = intValues
            .Concat(varcharValues)
            .Concat(textValues)
            .Concat(decimalValues)
            .Concat(dateTimeValues);
        return await AsyncExecuter.ToListAsync(combinedQuery);
    }

    public async Task<PagedResult<ProductAttributeValueDto>> GetProductAttributesAsync(ProductAttributeListFilterDto input)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

        var attributeIntQuery = (await _productAttributeIntRepository.GetQueryableAsync())
             .Where(x => x.ProductId == input.ProductId);
        var attributeVarcharQuery = (await _productAttributeVarcharRepository.GetQueryableAsync())
            .Where(x => x.ProductId == input.ProductId);
        var attributeTextQuery = (await _productAttributeTextRepository.GetQueryableAsync())
            .Where(x => x.ProductId == input.ProductId);
        var attributeDecimalQuery = (await _productAttributeDecimalRepository.GetQueryableAsync())
            .Where(x => x.ProductId == input.ProductId);
        var attributeDateTimeQuery = (await _productAttributeDateTimeRepository.GetQueryableAsync())
            .Where(x => x.ProductId == input.ProductId);

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
                    where (adate == null || adate.ProductId == input.ProductId)
                        && (adecimal == null || adecimal.ProductId == input.ProductId)
                         && (aint == null || aint.ProductId == input.ProductId)
                          && (aVarchar == null || aVarchar.ProductId == input.ProductId)
                           && (aText == null || aText.ProductId == input.ProductId)
                    select new ProductAttributeValueDto()
                    {
                        Label = a.Label,
                        AttributeId = a.Id,
                        DataType = a.DataType,
                        Code = a.Code,
                        ProductId = input.ProductId,
                        DateTimeValue = adate != null ? adate.Value : null,
                        DecimalValue = adecimal != null ? adecimal.Value : null,
                        IntValue = aint != null ? aint.Value : null,
                        TextValue = aText != null ? aText.Value : null,
                        VarcharValue = aVarchar != null ? aVarchar.Value : null,
                        DateTimeId = adate != null ? adate.Id : null,
                        DecimalId = adecimal != null ? adecimal.Id : null,
                        IntId = aint != null ? aint.Id : null,
                        TextId = aText != null ? aText.Id : null,
                        VarcharId = aVarchar != null ? aVarchar.Id : null,
                    };
        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.Label).Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize));
        return new PagedResult<ProductAttributeValueDto>(data, totalCount, input.CurrentPage, input.PageSize);
    }

}

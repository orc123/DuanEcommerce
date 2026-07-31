using DuanEcommerce.ProductCategories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace DuanEcommerce.Products;

public class ProductManager(
    IRepository<Product, Guid> _productRepository, 
    IRepository<ProductCategory, Guid> _productCategoryRepository
    ) : DomainService
{
    public async Task<Product> CreateAsync(
        Guid manufacturerId,
            string name, string code, string slug,
            ProductType productType, string sKU,
            int sortOrder, bool visibility,
            bool isActive, Guid categoryId,
            string seoMetaDescription, string description,
            string thumbnailPicture, double sellPrice)
    {
        if (await _productRepository.AnyAsync(x => x.Name == name))
        {
            throw new UserFriendlyException("Tên sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductNameAlreadyExists);
        }
        if (await _productRepository.AnyAsync(x => x.Code == code))
        {
            throw new UserFriendlyException("Mã sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductCodeAlreadyExists);
        }
        if (await _productRepository.AnyAsync(x => x.SKU == sKU))
        {
            throw new UserFriendlyException("Mã SKU sản phẩm đã tồn tại", DuanEcommerceDomainErrorCodes.ProductSKUAlreadyExists);
        }

        var category = await _productCategoryRepository.GetAsync(categoryId);

        return new Product(Guid.NewGuid(), manufacturerId, name, code, slug, productType, sKU, sortOrder,
                visibility, isActive, categoryId, seoMetaDescription, description, thumbnailPicture, sellPrice, category?.Name, category?.Slug);
    }
}

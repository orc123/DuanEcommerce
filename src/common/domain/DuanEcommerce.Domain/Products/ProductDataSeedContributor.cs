using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuanEcommerce.Manufacturers;
using DuanEcommerce.ProductCategories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.Products;

public class ProductDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
    private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;

    public ProductDataSeedContributor(
        IRepository<Product, Guid> productRepository,
        IRepository<ProductCategory, Guid> productCategoryRepository,
        IRepository<Manufacturer, Guid> manufacturerRepository)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _manufacturerRepository = manufacturerRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _productRepository.GetCountAsync() > 0)
        {
            return;
        }

        // Lấy thông tin Category & Manufacturer đã được seed trước đó
        var phoneCategory = await _productCategoryRepository.FirstOrDefaultAsync(x => x.Code == "DIEN_THOAI");
        var laptopCategory = await _productCategoryRepository.FirstOrDefaultAsync(x => x.Code == "LAPTOP");

        var appleManufacturer = await _manufacturerRepository.FirstOrDefaultAsync(x => x.Code == "APPLE");
        var samsungManufacturer = await _manufacturerRepository.FirstOrDefaultAsync(x => x.Code == "SAMSUNG");

        var products = new List<Product>();

        if (phoneCategory != null && appleManufacturer != null)
        {
            products.Add(new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Code = "IP15PM-256",
                Slug = "iphone-15-pro-max-256gb",
                SKU = "IP15PM-256-VN",
                ProductType = ProductType.Single,
                SortOrder = 1,
                Visibility = true,
                IsActive = true,
                CategoryId = phoneCategory.Id,
                CategoryName = phoneCategory.Name,
                CategorySlug = phoneCategory.Slug,
                ManufacturerId = appleManufacturer.Id,
                SellPrice = 29990000,
                Description = "Điện thoại iPhone 15 Pro Max 256GB chính hãng VN/A",
                SeoMetaDescription = "Mua iPhone 15 Pro Max 256GB chính hãng giá tốt",
                ThumbnailPicture = "iphone-15-pro-max.png"
            });
        }

        if (phoneCategory != null && samsungManufacturer != null)
        {
            products.Add(new Product
            {
                Name = "Samsung Galaxy S24 Ultra 512GB",
                Code = "S24U-512",
                Slug = "samsung-galaxy-s24-ultra-512gb",
                SKU = "S24U-512-VN",
                ProductType = ProductType.Single,
                SortOrder = 2,
                Visibility = true,
                IsActive = true,
                CategoryId = phoneCategory.Id,
                CategoryName = phoneCategory.Name,
                CategorySlug = phoneCategory.Slug,
                ManufacturerId = samsungManufacturer.Id,
                SellPrice = 27990000,
                Description = "Điện thoại Samsung Galaxy S24 Ultra 512GB tích hợp Galaxy AI",
                SeoMetaDescription = "Mua Samsung Galaxy S24 Ultra 512GB giá tốt",
                ThumbnailPicture = "samsung-s24-ultra.png"
            });
        }

        if (laptopCategory != null && appleManufacturer != null)
        {
            products.Add(new Product
            {
                Name = "MacBook Air M2 13 inch 8GB 256GB",
                Code = "MBA-M2-256",
                Slug = "macbook-air-m2-13-inch-8gb-256gb",
                SKU = "MBA-M2-256-VN",
                ProductType = ProductType.Single,
                SortOrder = 3,
                Visibility = true,
                IsActive = true,
                CategoryId = laptopCategory.Id,
                CategoryName = laptopCategory.Name,
                CategorySlug = laptopCategory.Slug,
                ManufacturerId = appleManufacturer.Id,
                SellPrice = 24490000,
                Description = "Laptop Apple MacBook Air M2 2022 8GB/256GB",
                SeoMetaDescription = "Mua MacBook Air M2 256GB chính hãng",
                ThumbnailPicture = "macbook-air-m2.png"
            });
        }

        if (products.Count > 0)
        {
            await _productRepository.InsertManyAsync(products);
        }
    }
}

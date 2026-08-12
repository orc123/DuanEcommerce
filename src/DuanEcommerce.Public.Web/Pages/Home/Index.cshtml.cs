using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Products;
using DuanEcommerce.Public.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace DuanEcommerce.Public.Web.Pages;

public class IndexModel(IProductCategoriesAppService productCategoriesAppService,
    IProductsAppService productsAppService, IDistributedCache<HomeCacheItem> distributedCache) : PublicPageModel
{
    private readonly IDistributedCache<HomeCacheItem> _distributedCache = distributedCache;
    private readonly IProductCategoriesAppService _productCategoriesAppService = productCategoriesAppService;
    private readonly IProductsAppService _productsAppService = productsAppService;

    public List<ProductCategoryDto> Categories { set; get; }
    public List<ProductDto> TopSellerProducts { set; get; }

    public async Task OnGetAsync()
    {
        var cacheItem = await _distributedCache.GetOrAddAsync(DuanEcommercePublicConsts.CacheKeys.HomeData, async () =>
        {
            var allCategories = await _productCategoriesAppService.GetListAllAsync();
            var rootCategories = allCategories.Where(x => x.ParentId == null).ToList();
            foreach (var category in rootCategories)
            {
                category.Children = rootCategories.Where(x => x.ParentId == category.Id).ToList();
            }

            var topSellerProducts = await _productsAppService.GetListTopSellerAsync(10);
            return new HomeCacheItem()
            {
                TopSellerProducts = topSellerProducts,
                Categories = rootCategories
            };

        },
        () => new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
        });

        TopSellerProducts = cacheItem.TopSellerProducts;
        Categories = cacheItem.Categories;

    }
}

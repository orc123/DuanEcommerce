using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace DuanEcommerce.Public.Web.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly IProductCategoriesAppService _productCategoriesAppService;
    private readonly IDistributedCache<HeaderCacheItem> _distributedCache;

    public HeaderViewComponent(IProductCategoriesAppService productCategoriesAppService, IDistributedCache<HeaderCacheItem> distributedCache)
    {
        _productCategoriesAppService = productCategoriesAppService;
        _distributedCache = distributedCache;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cacheItem = await _distributedCache.GetOrAddAsync(
            DuanEcommercePublicConsts.CacheKeys.HeaderData, async () =>
            {
                var model = await _productCategoriesAppService.GetListAllAsync();
                return new HeaderCacheItem
                {
                    Categories = model
                };
            },
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            }
        );
        return View(cacheItem.Categories);
    }
}

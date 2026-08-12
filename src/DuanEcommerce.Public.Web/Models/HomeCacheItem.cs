using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Products;
using System.Collections.Generic;

namespace DuanEcommerce.Public.Web.Models;

public class HomeCacheItem
{
    public List<ProductCategoryDto> Categories { get; set; }
    public List<ProductDto> TopSellerProducts { get; set; }
}

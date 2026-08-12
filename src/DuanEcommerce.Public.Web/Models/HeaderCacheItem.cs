using DuanEcommerce.Public.ProductCategories;
using System.Collections.Generic;

namespace DuanEcommerce.Public.Web.Models;

public class HeaderCacheItem
{
    public List<ProductCategoryDto> Categories { get; set; }
}

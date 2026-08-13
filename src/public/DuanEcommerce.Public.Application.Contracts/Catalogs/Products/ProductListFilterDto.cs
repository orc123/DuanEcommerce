using System;

namespace DuanEcommerce.Public.Products;

public class ProductListFilterDto : BaseListFilterDto
{
    public Guid? CategoryId { get; set; }
}

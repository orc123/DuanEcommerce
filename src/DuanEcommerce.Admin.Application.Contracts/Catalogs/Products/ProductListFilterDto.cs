using System;

namespace DuanEcommerce.Admin.Products;

public class ProductListFilterDto : BaseListFilterDto
{
    public Guid? CategoryId { get; set; }
}

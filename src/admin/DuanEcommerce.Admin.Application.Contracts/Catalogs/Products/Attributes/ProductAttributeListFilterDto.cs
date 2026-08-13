using System;

namespace DuanEcommerce.Admin.Products.Attributes;

public class ProductAttributeListFilterDto : BaseListFilterDto
{
    public Guid ProductId { get; set; }
}

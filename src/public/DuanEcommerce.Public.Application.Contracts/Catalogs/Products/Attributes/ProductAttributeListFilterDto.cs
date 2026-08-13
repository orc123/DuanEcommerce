using System;

namespace DuanEcommerce.Public.Products.Attributes;

public class ProductAttributeListFilterDto : BaseListFilterDto
{
    public Guid ProductId { get; set; }
}

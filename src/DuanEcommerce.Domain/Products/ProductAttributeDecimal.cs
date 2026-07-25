using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class ProductAttributeDecimal : Entity<Guid>
{
    public Guid AttributeId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }

}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class ProductLink : Entity
{
    public Guid ProductId { get; set; }
    public Guid LinkedProductId { get; set; }
    public override object?[] GetKeys()
    {
        return new object[] { ProductId, LinkedProductId };
    }
}

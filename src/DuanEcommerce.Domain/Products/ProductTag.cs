using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class ProductTag : Entity
{
    public Guid ProductId { get; set; }
    public string TagId { get; set; }

    public override object?[] GetKeys()
    {
        return new object[] { ProductId, TagId };
    }
}

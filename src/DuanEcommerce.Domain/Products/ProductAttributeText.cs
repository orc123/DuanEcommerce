using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class ProductAttributeText : Entity<Guid>
{
    public Guid AttributeId { get; set; }
    public Guid ProductId { get; set; }
    public string Value { get; set; }

}

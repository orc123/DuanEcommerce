using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class Tag : Entity<string>
{
    public string Name { get; set; }
}

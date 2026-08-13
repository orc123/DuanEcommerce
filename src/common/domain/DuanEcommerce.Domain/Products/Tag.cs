using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Products;

public class Tag : Entity<string>
{
    public string Name { get; set; }
}

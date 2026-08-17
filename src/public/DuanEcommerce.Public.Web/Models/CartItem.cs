using DuanEcommerce.Public.Products;

namespace DuanEcommerce.Public.Web.Models;

public class CartItem
{
    public ProductDto Product { get; set; } = new ProductDto();
    public int Quantity { get; set; } = 0;
}

using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DuanEcommerce.Public.Web.Pages.Products;

public class DetailModel : PageModel
{
    private readonly IProductsAppService _productsAppService;
    private readonly IProductCategoriesAppService _productCategoriesAppService;
    public DetailModel(IProductsAppService productsAppService,
        IProductCategoriesAppService productCategoriesAppService)
    {
        _productsAppService = productsAppService;
        _productCategoriesAppService = productCategoriesAppService;
    }
    public ProductCategoryDto Category { get; set; }
    public ProductDto Product { get; set; }
    public async Task OnGetAsync(string categorySlug, string slug)
    {
        Category = await _productCategoriesAppService.GetBySlugAsync(categorySlug);
        Product = await _productsAppService.GetBySlugAsync(slug);
    }
}
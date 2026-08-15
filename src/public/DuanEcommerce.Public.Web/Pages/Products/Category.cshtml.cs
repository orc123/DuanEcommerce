using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuanEcommerce.Public.Web.Pages.Products;

public class CategoryModel(IProductCategoriesAppService productCategoriesAppService,
    IProductsAppService productsAppService) : PageModel
{
    public ProductCategoryDto? Category { set; get; }

    public List<ProductCategoryDto> Categories { set; get; }
    public PagedResult<ProductDto> ProductData { set; get; }

    private readonly IProductCategoriesAppService _productCategoriesAppService = productCategoriesAppService;
    private readonly IProductsAppService _productsAppService = productsAppService;

    public async Task OnGetAsync(string code, int page = 1)
    {
        Category = await _productCategoriesAppService.GetByCodeAsync(code);
        Categories = await _productCategoriesAppService.GetListAllAsync();
        ProductData = await _productsAppService.GetListFilterAsync(new ProductListFilterDto()
        {
            CurrentPage = page
        });
    }
}

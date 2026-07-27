using DuanEcommerce.Admin.ProductCategories;
using DuanEcommerce.Admin.Products;
using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace DuanEcommerce.Admin;

[Mapper]
public partial class ProductCategoryToProductCategoryDtoMapper : MapperBase<ProductCategory,  ProductCategoryDto>
{
    public override partial ProductCategoryDto Map(ProductCategory source);

    public override partial void Map(ProductCategory source, ProductCategoryDto destination);
}

[Mapper]
public partial class CreateUpdateProductCategoryDtoToProductCategoryMapper : MapperBase<CreateUpdateProductCategoryDto, ProductCategory>
{
    public override partial ProductCategory Map(CreateUpdateProductCategoryDto source);

    public override partial void Map(CreateUpdateProductCategoryDto source, ProductCategory destination);
}

[Mapper]
public partial class ProductToProductDtoMapper : MapperBase<Product, ProductDto>
{
    public override partial ProductDto Map(Product source);

    public override partial void Map(Product source, ProductDto destination);
}

[Mapper]
public partial class CreateUpdateProductToProductMapper : MapperBase<CreateUpdateProductDto, Product>
{
    public override partial Product Map(CreateUpdateProductDto source);

    public override partial void Map(CreateUpdateProductDto source, Product destination);
}
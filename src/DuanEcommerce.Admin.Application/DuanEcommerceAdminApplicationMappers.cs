using DuanEcommerce.Admin.ProductCategories;
using DuanEcommerce.ProductCategories;
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
public partial class ProductCategoryToProductCategoryInListDtoMapper : MapperBase<ProductCategory, ProductCategoryInListDto>
{
    public override partial ProductCategoryInListDto Map(ProductCategory source);

    public override partial void Map(ProductCategory source, ProductCategoryInListDto destination);
}

[Mapper]
public partial class CreateUpdateProductCategoryDtoToProductCategoryMapper : MapperBase<CreateUpdateProductCategoryDto, ProductCategory>
{
    public override partial ProductCategory Map(CreateUpdateProductCategoryDto source);

    public override partial void Map(CreateUpdateProductCategoryDto source, ProductCategory destination);
}
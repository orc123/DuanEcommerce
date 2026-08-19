using DuanEcommerce.Manufacturers;
using DuanEcommerce.Orders;
using DuanEcommerce.ProductAttributes;
using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using DuanEcommerce.Public.Manufacturers;
using DuanEcommerce.Public.Orders;
using DuanEcommerce.Public.ProductAttributes;
using DuanEcommerce.Public.ProductCategories;
using DuanEcommerce.Public.Products;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace DuanEcommerce.Public;

/*
 * You can add your own mappings here.
 * [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
 * public partial class PublicApplicationMappers : MapperBase<BookDto, CreateUpdateBookDto>
 * {
 *    public override partial CreateUpdateBookDto Map(BookDto source);
 *
 *    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
 * }
 */

[Mapper]
public partial class ProductCategoryToProductCategoryDtoMapper : MapperBase<ProductCategory, ProductCategoryDto>
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
public partial class ManufacturerToManufacturerDtoMapper : MapperBase<Manufacturer, ManufacturerDto>
{
    public override partial ManufacturerDto Map(Manufacturer source);

    public override partial void Map(Manufacturer source, ManufacturerDto destination);
}

[Mapper]
public partial class CreateUpdateManufacturerDtoToManufacturer : MapperBase<CreateUpdateManufacturerDto, Manufacturer>
{
    public override partial Manufacturer Map(CreateUpdateManufacturerDto source);

    public override partial void Map(CreateUpdateManufacturerDto source, Manufacturer destination);
}

[Mapper]
public partial class ProductAttributeToProductAttributeDtoMapper : MapperBase<ProductAttribute, ProductAttributeDto>
{
    public override partial ProductAttributeDto Map(ProductAttribute source);

    public override partial void Map(ProductAttribute source, ProductAttributeDto destination);
}

[Mapper]
public partial class CreateUpdateProductAttributeDtoToProductAttribute : MapperBase<CreateUpdateProductAttributeDto, ProductAttribute>
{
    public override partial ProductAttribute Map(CreateUpdateProductAttributeDto source);

    public override partial void Map(CreateUpdateProductAttributeDto source, ProductAttribute destination);
}

[Mapper]
public partial class OrderToOrderDtoMapper : MapperBase<Order, OrderDto>
{
    public override partial OrderDto Map(Order source);

    public override partial void Map(Order source, OrderDto destination);
}
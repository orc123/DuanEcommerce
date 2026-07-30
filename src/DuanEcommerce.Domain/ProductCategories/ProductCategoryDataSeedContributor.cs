using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.ProductCategories;

public class ProductCategoryDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;

    public ProductCategoryDataSeedContributor(IRepository<ProductCategory, Guid> productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _productCategoryRepository.GetCountAsync() > 0)
        {
            return;
        }

        var phoneCategory = new ProductCategory
        {
            Name = "Điện thoại",
            Code = "DIEN_THOAI",
            Slug = "dien-thoai",
            SortOrder = 1,
            CoverPicture = "dien-thoai.png",
            Visibility = true,
            IsActive = true,
            ParentId = null,
            SeoMetaDescription = "Danh mục Điện thoại thông minh"
        };

        var laptopCategory = new ProductCategory
        {
            Name = "Laptop",
            Code = "LAPTOP",
            Slug = "laptop",
            SortOrder = 2,
            CoverPicture = "laptop.png",
            Visibility = true,
            IsActive = true,
            ParentId = null,
            SeoMetaDescription = "Danh mục máy tính xách tay Laptop"
        };

        var tabletCategory = new ProductCategory
        {
            Name = "Máy tính bảng",
            Code = "MAY_TINH_BANG",
            Slug = "may-tinh-bang",
            SortOrder = 3,
            CoverPicture = "tablet.png",
            Visibility = true,
            IsActive = true,
            ParentId = null,
            SeoMetaDescription = "Danh mục Máy tính bảng Tablet"
        };

        var accessoryCategory = new ProductCategory
        {
            Name = "Phụ kiện",
            Code = "PHU_KIEN",
            Slug = "phu-kien",
            SortOrder = 4,
            CoverPicture = "phu-kien.png",
            Visibility = true,
            IsActive = true,
            ParentId = null,
            SeoMetaDescription = "Danh mục Phụ kiện điện tử"
        };

        await _productCategoryRepository.InsertManyAsync(new[]
        {
            phoneCategory,
            laptopCategory,
            tabletCategory,
            accessoryCategory
        });
    }
}

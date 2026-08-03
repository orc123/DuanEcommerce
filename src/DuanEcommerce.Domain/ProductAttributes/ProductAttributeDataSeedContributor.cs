using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.ProductAttributes;

public class ProductAttributeDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ProductAttribute, Guid> _productAttributeRepository;

    public ProductAttributeDataSeedContributor(IRepository<ProductAttribute, Guid> productAttributeRepository)
    {
        _productAttributeRepository = productAttributeRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _productAttributeRepository.GetCountAsync() > 0)
        {
            return;
        }

        var attributes = new[]
        {
            new ProductAttribute
            {
                Code = "COLOR",
                Label = "Màu sắc",
                DataType = AttributeType.Varchar,
                SortOrder = 1,
                Visibility = true,
                IsActive = true,
                IsRequired = false,
                IsUnique = false,
                Note = "Thuộc tính màu sắc của sản phẩm"
            },
            new ProductAttribute
            {
                Code = "STORAGE_CAPACITY",
                Label = "Dung lượng bộ nhớ",
                DataType = AttributeType.Varchar,
                SortOrder = 2,
                Visibility = true,
                IsActive = true,
                IsRequired = false,
                IsUnique = false,
                Note = "Ví dụ: 128GB, 256GB, 512GB, 1TB"
            },
            new ProductAttribute
            {
                Code = "RAM",
                Label = "Dung lượng RAM",
                DataType = AttributeType.Varchar,
                SortOrder = 3,
                Visibility = true,
                IsActive = true,
                IsRequired = false,
                IsUnique = false,
                Note = "Ví dụ: 8GB, 16GB, 32GB"
            },
            new ProductAttribute
            {
                Code = "SCREEN_SIZE",
                Label = "Kích thước màn hình (inch)",
                DataType = AttributeType.Decimal,
                SortOrder = 4,
                Visibility = true,
                IsActive = true,
                IsRequired = false,
                IsUnique = false,
                Note = "Ví dụ: 6.1, 6.7, 13.3"
            },
            new ProductAttribute
            {
                Code = "WARRANTY_MONTHS",
                Label = "Thời gian bảo hành (tháng)",
                DataType = AttributeType.Int,
                SortOrder = 5,
                Visibility = true,
                IsActive = true,
                IsRequired = false,
                IsUnique = false,
                Note = "Số tháng bảo hành chính hãng"
            }
        };

        await _productAttributeRepository.InsertManyAsync(attributes);
    }
}

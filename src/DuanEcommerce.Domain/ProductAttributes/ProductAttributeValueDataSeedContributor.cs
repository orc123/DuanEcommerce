using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuanEcommerce.Products;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.ProductAttributes;

public class ProductAttributeValueDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ProductAttribute, Guid> _productAttributeRepository;
    private readonly IRepository<Product, Guid> _productRepository;

    private readonly IRepository<ProductAttributeVarchar, Guid> _attributeVarcharRepository;
    private readonly IRepository<ProductAttributeInt, Guid> _attributeIntRepository;
    private readonly IRepository<ProductAttributeDecimal, Guid> _attributeDecimalRepository;
    private readonly IRepository<ProductAttributeDateTime, Guid> _attributeDateTimeRepository;
    private readonly IRepository<ProductAttributeText, Guid> _attributeTextRepository;

    public ProductAttributeValueDataSeedContributor(
        IRepository<ProductAttribute, Guid> productAttributeRepository,
        IRepository<Product, Guid> productRepository,
        IRepository<ProductAttributeVarchar, Guid> attributeVarcharRepository,
        IRepository<ProductAttributeInt, Guid> attributeIntRepository,
        IRepository<ProductAttributeDecimal, Guid> attributeDecimalRepository,
        IRepository<ProductAttributeDateTime, Guid> attributeDateTimeRepository,
        IRepository<ProductAttributeText, Guid> attributeTextRepository)
    {
        _productAttributeRepository = productAttributeRepository;
        _productRepository = productRepository;
        _attributeVarcharRepository = attributeVarcharRepository;
        _attributeIntRepository = attributeIntRepository;
        _attributeDecimalRepository = attributeDecimalRepository;
        _attributeDateTimeRepository = attributeDateTimeRepository;
        _attributeTextRepository = attributeTextRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        // Kiểm tra xem đã seed giá trị thuộc tính chưa
        if (await _attributeVarcharRepository.GetCountAsync() > 0 ||
            await _attributeIntRepository.GetCountAsync() > 0 ||
            await _attributeDecimalRepository.GetCountAsync() > 0)
        {
            return;
        }

        var iphone = await _productRepository.FirstOrDefaultAsync(p => p.Code == "IP15PM-256");
        var macbook = await _productRepository.FirstOrDefaultAsync(p => p.Code == "MBA-M2-256");

        var colorAttr = await _productAttributeRepository.FirstOrDefaultAsync(a => a.Code == "COLOR");
        var storageAttr = await _productAttributeRepository.FirstOrDefaultAsync(a => a.Code == "STORAGE_CAPACITY");
        var ramAttr = await _productAttributeRepository.FirstOrDefaultAsync(a => a.Code == "RAM");
        var screenSizeAttr = await _productAttributeRepository.FirstOrDefaultAsync(a => a.Code == "SCREEN_SIZE");
        var warrantyAttr = await _productAttributeRepository.FirstOrDefaultAsync(a => a.Code == "WARRANTY_MONTHS");

        // 1. Seed ProductAttributeVarchar (Màu sắc, Dung lượng, RAM)
        var varcharValues = new List<ProductAttributeVarchar>();
        if (iphone != null)
        {
            if (colorAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), colorAttr.Id, iphone.Id, "Titan Tự Nhiên"));
            if (storageAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), storageAttr.Id, iphone.Id, "256GB"));
            if (ramAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), ramAttr.Id, iphone.Id, "8GB"));
        }

        if (macbook != null)
        {
            if (colorAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), colorAttr.Id, macbook.Id, "Starlight"));
            if (storageAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), storageAttr.Id, macbook.Id, "256GB"));
            if (ramAttr != null)
                varcharValues.Add(new ProductAttributeVarchar(Guid.NewGuid(), ramAttr.Id, macbook.Id, "8GB"));
        }

        if (varcharValues.Count > 0)
        {
            await _attributeVarcharRepository.InsertManyAsync(varcharValues);
        }

        // 2. Seed ProductAttributeDecimal (Kích thước màn hình)
        var decimalValues = new List<ProductAttributeDecimal>();
        if (iphone != null && screenSizeAttr != null)
        {
            decimalValues.Add(new ProductAttributeDecimal(Guid.NewGuid(), screenSizeAttr.Id, iphone.Id, 6.7m));
        }
        if (macbook != null && screenSizeAttr != null)
        {
            decimalValues.Add(new ProductAttributeDecimal(Guid.NewGuid(), screenSizeAttr.Id, macbook.Id, 13.6m));
        }

        if (decimalValues.Count > 0)
        {
            await _attributeDecimalRepository.InsertManyAsync(decimalValues);
        }

        // 3. Seed ProductAttributeInt (Số tháng bảo hành)
        var intValues = new List<ProductAttributeInt>();
        if (iphone != null && warrantyAttr != null)
        {
            intValues.Add(new ProductAttributeInt(Guid.NewGuid(), warrantyAttr.Id, iphone.Id, 12));
        }
        if (macbook != null && warrantyAttr != null)
        {
            intValues.Add(new ProductAttributeInt(Guid.NewGuid(), warrantyAttr.Id, macbook.Id, 12));
        }

        if (intValues.Count > 0)
        {
            await _attributeIntRepository.InsertManyAsync(intValues);
        }

        // 4. Seed ProductAttributeText & ProductAttributeDateTime (Mô tả chi tiết & Ngày ra mắt)
        var textValues = new List<ProductAttributeText>();
        var dateTimeValues = new List<ProductAttributeDateTime>();

        if (iphone != null)
        {
            if (colorAttr != null)
            {
                textValues.Add(new ProductAttributeText(
                    Guid.NewGuid(), 
                    colorAttr.Id, 
                    iphone.Id, 
                    "Khung vỏ Titan cấp độ 5 siêu nhẹ và bền bỉ, mặt lưng kính nhám sang trọng."));
            }

            if (warrantyAttr != null)
            {
                dateTimeValues.Add(new ProductAttributeDateTime(
                    Guid.NewGuid(), 
                    warrantyAttr.Id, 
                    iphone.Id, 
                    new DateTime(2023, 9, 15)));
            }
        }

        if (textValues.Count > 0)
        {
            await _attributeTextRepository.InsertManyAsync(textValues);
        }

        if (dateTimeValues.Count > 0)
        {
            await _attributeDateTimeRepository.InsertManyAsync(dateTimeValues);
        }
    }
}

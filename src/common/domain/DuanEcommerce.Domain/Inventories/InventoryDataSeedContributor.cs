using System;
using System.Threading.Tasks;
using DuanEcommerce.Products;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.Inventories;

public class InventoryDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Inventory, Guid> _inventoryRepository;
    private readonly IRepository<Product, Guid> _productRepository;

    public InventoryDataSeedContributor(
        IRepository<Inventory, Guid> inventoryRepository,
        IRepository<Product, Guid> productRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _inventoryRepository.GetCountAsync() > 0)
        {
            return;
        }

        var products = await _productRepository.GetListAsync();
        foreach (var product in products)
        {
            await _inventoryRepository.InsertAsync(new Inventory
            {
                ProductId = product.Id,
                SKU = product.SKU,
                StockQuantity = 100 // Mặc định mỗi sản phẩm khởi tạo có 100 cái trong kho
            });
        }
    }
}

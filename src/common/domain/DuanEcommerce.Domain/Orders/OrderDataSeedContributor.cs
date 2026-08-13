using System;
using System.Threading.Tasks;
using DuanEcommerce.Products;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.Orders;

public class OrderDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<Product, Guid> _productRepository;

    public OrderDataSeedContributor(
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Product, Guid> productRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _orderRepository.GetCountAsync() > 0)
        {
            return;
        }

        var product = await _productRepository.FirstOrDefaultAsync(p => p.Code == "IP15PM-256");
        if (product == null)
        {
            return;
        }

        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Code = "ORD-2026-0001",
            Status = OrderStatus.New,
            PaymentMethod = PaymentMethod.COD,
            ShippingFee = 30000,
            Tax = 0,
            Subtotal = product.SellPrice,
            Discount = 0,
            Total = product.SellPrice + 30000,
            GrandTotal = product.SellPrice + 30000,
            CustomerName = "Nguyen Van A",
            CustomerPhoneNumber = "0987654321",
            CustomerAddress = "123 Le Loi, Quan 1, TP. HCM"
        };

        // Gán Id trực tiếp do Entity có Id từ FullAuditedAggregateRoot<Guid>
        typeof(Order).GetProperty("Id")?.SetValue(order, orderId);

        await _orderRepository.InsertAsync(order);

        await _orderItemRepository.InsertAsync(new OrderItem
        {
            OrderId = orderId,
            ProductId = product.Id,
            SKU = product.SKU,
            Quantity = 1,
            Price = product.SellPrice
        });
    }
}

using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.Promotions;

public class PromotionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Promotion, Guid> _promotionRepository;

    public PromotionDataSeedContributor(IRepository<Promotion, Guid> promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _promotionRepository.GetCountAsync() > 0)
        {
            return;
        }

        var promotions = new[]
        {
            new Promotion
            {
                Name = "Khuyến mãi Khai Trương",
                CouponCode = "KHAITRUONG10",
                RequireUseCouponCode = true,
                ValidDate = DateTime.Now.AddDays(-7),
                ExpiredDate = DateTime.Now.AddMonths(1),
                DiscountAmount = 10,
                DiscountUnit = DiscountUnit.Percentage,
                LimitedUsageTimes = false,
                MaximumDiscountAmount = 500000,
                IsActive = true
            },
            new Promotion
            {
                Name = "Giảm trực tiếp 100k cho đơn từ 2 triệu",
                CouponCode = "GIAM100K",
                RequireUseCouponCode = true,
                ValidDate = DateTime.Now.AddDays(-1),
                ExpiredDate = DateTime.Now.AddMonths(2),
                DiscountAmount = 100000,
                DiscountUnit = DiscountUnit.MoneyAmount,
                LimitedUsageTimes = true,
                MaximumDiscountAmount = 100000,
                IsActive = true
            }
        };

        await _promotionRepository.InsertManyAsync(promotions);
    }
}

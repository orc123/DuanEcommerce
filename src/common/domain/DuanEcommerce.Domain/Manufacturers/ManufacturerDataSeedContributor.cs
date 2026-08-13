using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace DuanEcommerce.Manufacturers;

public class ManufacturerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;

    public ManufacturerDataSeedContributor(IRepository<Manufacturer, Guid> manufacturerRepository)
    {
        _manufacturerRepository = manufacturerRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _manufacturerRepository.GetCountAsync() > 0)
        {
            return;
        }

        var manufacturers = new[]
        {
            new Manufacturer
            {
                Name = "Apple",
                Code = "APPLE",
                Slug = "apple",
                CoverPicture = "apple-logo.png",
                Visibility = true,
                IsActive = true,
                Country = "USA"
            },
            new Manufacturer
            {
                Name = "Samsung",
                Code = "SAMSUNG",
                Slug = "samsung",
                CoverPicture = "samsung-logo.png",
                Visibility = true,
                IsActive = true,
                Country = "South Korea"
            },
            new Manufacturer
            {
                Name = "Sony",
                Code = "SONY",
                Slug = "sony",
                CoverPicture = "sony-logo.png",
                Visibility = true,
                IsActive = true,
                Country = "Japan"
            },
            new Manufacturer
            {
                Name = "Xiaomi",
                Code = "XIAOMI",
                Slug = "xiaomi",
                CoverPicture = "xiaomi-logo.png",
                Visibility = true,
                IsActive = true,
                Country = "China"
            },
            new Manufacturer
            {
                Name = "Asus",
                Code = "ASUS",
                Slug = "asus",
                CoverPicture = "asus-logo.png",
                Visibility = true,
                IsActive = true,
                Country = "Taiwan"
            }
        };

        await _manufacturerRepository.InsertManyAsync(manufacturers);
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DuanEcommerce.Data;
using Volo.Abp.DependencyInjection;

namespace DuanEcommerce.EntityFrameworkCore;

public class EntityFrameworkCoreDuanEcommerceDbSchemaMigrator
    : IDuanEcommerceDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDuanEcommerceDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DuanEcommerceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DuanEcommerceDbContext>()
            .Database
            .MigrateAsync();
    }
}

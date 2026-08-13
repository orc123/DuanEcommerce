using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DuanEcommerce.Data;

/* This is used if database provider does't define
 * IDuanEcommerceDbSchemaMigrator implementation.
 */
public class NullDuanEcommerceDbSchemaMigrator : IDuanEcommerceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

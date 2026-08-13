using System.Threading.Tasks;

namespace DuanEcommerce.Data;

public interface IDuanEcommerceDbSchemaMigrator
{
    Task MigrateAsync();
}

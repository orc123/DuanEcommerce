using DuanEcommerce.Admin;
using DuanEcommerce.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DuanEcommerce.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DuanEcommerceEntityFrameworkCoreModule),
    typeof(DuanEcommerceAdminApplicationContractsModule)
)]
public class DuanEcommerceDbMigratorModule : AbpModule
{
}

using DuanEcommerce.Admin;
using Volo.Abp.Modularity;

namespace DuanEcommerce;

[DependsOn(
    typeof(DuanEcommerceAdminApplicationModule),
    typeof(DuanEcommerceDomainTestModule)
)]
public class DuanEcommerceAdminApplicationTestModule : AbpModule
{

}

using Volo.Abp.Modularity;

namespace DuanEcommerce;

[DependsOn(
    typeof(DuanEcommerceDomainModule),
    typeof(DuanEcommerceTestBaseModule)
)]
public class DuanEcommerceDomainTestModule : AbpModule
{

}

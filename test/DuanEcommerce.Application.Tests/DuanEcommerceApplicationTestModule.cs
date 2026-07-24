using Volo.Abp.Modularity;

namespace DuanEcommerce;

[DependsOn(
    typeof(DuanEcommerceApplicationModule),
    typeof(DuanEcommerceDomainTestModule)
)]
public class DuanEcommerceApplicationTestModule : AbpModule
{

}

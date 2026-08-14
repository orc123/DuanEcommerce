using Volo.Abp.Modularity;

namespace DuanEcommerce.Public;

[DependsOn(
    typeof(PublicApplicationModule),
    typeof(DuanEcommerceDomainTestModule)
)]
public class PublicApplicationTestModule : AbpModule
{

}

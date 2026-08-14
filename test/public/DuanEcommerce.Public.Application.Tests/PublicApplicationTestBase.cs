using Volo.Abp.Modularity;

namespace DuanEcommerce.Public;

public abstract class PublicApplicationTestBase<TStartupModule> : DuanEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

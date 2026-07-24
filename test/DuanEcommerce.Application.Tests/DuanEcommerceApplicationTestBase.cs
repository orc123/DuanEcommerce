using Volo.Abp.Modularity;

namespace DuanEcommerce;

public abstract class DuanEcommerceApplicationTestBase<TStartupModule> : DuanEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

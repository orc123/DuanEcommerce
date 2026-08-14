using Volo.Abp.Modularity;

namespace DuanEcommerce;

/* Inherit from this class for your domain layer tests. */
public abstract class DuanEcommerceDomainTestBase<TStartupModule> : DuanEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

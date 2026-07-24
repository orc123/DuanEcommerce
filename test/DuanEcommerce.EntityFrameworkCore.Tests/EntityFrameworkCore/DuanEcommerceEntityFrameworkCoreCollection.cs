using Xunit;

namespace DuanEcommerce.EntityFrameworkCore;

[CollectionDefinition(DuanEcommerceTestConsts.CollectionDefinitionName)]
public class DuanEcommerceEntityFrameworkCoreCollection : ICollectionFixture<DuanEcommerceEntityFrameworkCoreFixture>
{

}

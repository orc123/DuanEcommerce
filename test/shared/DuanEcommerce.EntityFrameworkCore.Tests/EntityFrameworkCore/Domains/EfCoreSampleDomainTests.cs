using DuanEcommerce.Samples;
using Xunit;

namespace DuanEcommerce.EntityFrameworkCore.Domains;

[Collection(DuanEcommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DuanEcommerceEntityFrameworkCoreTestModule>
{

}

using DuanEcommerce.Samples;
using Xunit;

namespace DuanEcommerce.EntityFrameworkCore.Applications;

[Collection(DuanEcommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DuanEcommerceEntityFrameworkCoreTestModule>
{

}

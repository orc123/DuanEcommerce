using DuanEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public;

/* Inherit your application services from this class.
 */
public abstract class PublicAppService : ApplicationService
{
    protected PublicAppService()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

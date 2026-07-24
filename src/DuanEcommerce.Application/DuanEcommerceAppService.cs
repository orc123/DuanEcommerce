using DuanEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace DuanEcommerce;

/* Inherit your application services from this class.
 */
public abstract class DuanEcommerceAppService : ApplicationService
{
    protected DuanEcommerceAppService()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

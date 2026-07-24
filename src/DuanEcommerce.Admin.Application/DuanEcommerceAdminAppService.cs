using DuanEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Admin;

/* Inherit your application services from this class.
 */
public abstract class DuanEcommerceAdminAppService : ApplicationService
{
    protected DuanEcommerceAdminAppService()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

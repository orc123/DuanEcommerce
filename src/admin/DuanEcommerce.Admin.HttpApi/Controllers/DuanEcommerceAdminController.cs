using DuanEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DuanEcommerce.Admin.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DuanEcommerceAdminController : AbpControllerBase
{
    protected DuanEcommerceAdminController()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

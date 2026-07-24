using DuanEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DuanEcommerce.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DuanEcommerceController : AbpControllerBase
{
    protected DuanEcommerceController()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

using DuanEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DuanEcommerce.Public.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PublicController : AbpControllerBase
{
    protected PublicController()
    {
        LocalizationResource = typeof(DuanEcommerceResource);
    }
}

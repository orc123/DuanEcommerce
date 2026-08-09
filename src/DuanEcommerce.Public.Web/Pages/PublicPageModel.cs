using DuanEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DuanEcommerce.Public.Web.Pages;

public abstract class PublicPageModel : AbpPageModel
{
    protected PublicPageModel()
    {
        LocalizationResourceType = typeof(DuanEcommerceResource);
    }
}

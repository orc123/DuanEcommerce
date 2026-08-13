using DuanEcommerce.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DuanEcommerce.Public.Web;

[Dependency(ReplaceServices = true)]
public class PublicBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DuanEcommerceResource> _localizer;

    public PublicBrandingProvider(IStringLocalizer<DuanEcommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

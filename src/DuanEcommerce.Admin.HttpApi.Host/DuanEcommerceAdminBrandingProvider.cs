using Microsoft.Extensions.Localization;
using DuanEcommerce.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DuanEcommerce.Admin;

[Dependency(ReplaceServices = true)]
public class DuanEcommerceAdminBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DuanEcommerceResource> _localizer;

    public DuanEcommerceAdminBrandingProvider(IStringLocalizer<DuanEcommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

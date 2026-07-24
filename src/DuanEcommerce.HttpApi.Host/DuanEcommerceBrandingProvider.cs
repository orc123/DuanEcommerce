using Microsoft.Extensions.Localization;
using DuanEcommerce.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DuanEcommerce;

[Dependency(ReplaceServices = true)]
public class DuanEcommerceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DuanEcommerceResource> _localizer;

    public DuanEcommerceBrandingProvider(IStringLocalizer<DuanEcommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

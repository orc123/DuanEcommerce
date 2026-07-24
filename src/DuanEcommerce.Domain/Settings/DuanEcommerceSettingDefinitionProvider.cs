using Volo.Abp.Settings;

namespace DuanEcommerce.Settings;

public class DuanEcommerceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DuanEcommerceSettings.MySetting1));
    }
}

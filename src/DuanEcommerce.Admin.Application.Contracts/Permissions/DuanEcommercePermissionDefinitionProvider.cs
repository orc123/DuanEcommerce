using DuanEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace DuanEcommerce.Admin.Permissions;

public class DuanEcommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DuanEcommercePermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(DuanEcommercePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DuanEcommerceResource>(name);
    }
}

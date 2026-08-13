using DuanEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace DuanEcommerce.Public.Permissions;

public class PublicPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PublicPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(PublicPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DuanEcommerceResource>(name);
    }
}

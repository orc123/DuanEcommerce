using DuanEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DuanEcommerce.Admin.Permissions;

public class DuanEcommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //Catalog
        var catalogGroup = context.AddGroup(DuanEcommercePermissions.CatalogGroupName, L("Permission:Catalog"));

        // Manufacturer
        var manufacturerPermission = catalogGroup.AddPermission(DuanEcommercePermissions.Manufacturer.Default, L("Permission:Catalog.Manufacturer"));
        manufacturerPermission.AddChild(DuanEcommercePermissions.Manufacturer.Create, L("Permission:Catalog.Manufacturer.Create"));
        manufacturerPermission.AddChild(DuanEcommercePermissions.Manufacturer.Update, L("Permission:Catalog.Manufacturer.Update"));
        manufacturerPermission.AddChild(DuanEcommercePermissions.Manufacturer.Delete, L("Permission:Catalog.Manufacturer.Delete"));

        // ProductCategory productCategory
        var productCategoryPermission = catalogGroup.AddPermission(DuanEcommercePermissions.ProductCategory.Default, L("Permission:Catalog.ProductCategory"));
        productCategoryPermission.AddChild(DuanEcommercePermissions.ProductCategory.Create, L("Permission:Catalog.ProductCategory.Create"));
        productCategoryPermission.AddChild(DuanEcommercePermissions.ProductCategory.Update, L("Permission:Catalog.ProductCategory.Update"));
        productCategoryPermission.AddChild(DuanEcommercePermissions.ProductCategory.Delete, L("Permission:Catalog.ProductCategory.Delete"));

        //Add product
        var productPermission = catalogGroup.AddPermission(DuanEcommercePermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(DuanEcommercePermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(DuanEcommercePermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(DuanEcommercePermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(DuanEcommercePermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //Add attribute
        var attributePermission = catalogGroup.AddPermission(DuanEcommercePermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(DuanEcommercePermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(DuanEcommercePermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(DuanEcommercePermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DuanEcommerceResource>(name);
    }
}

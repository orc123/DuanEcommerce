namespace DuanEcommerce.Admin.Permissions;

public static class DuanEcommercePermissions
{
    public const string SystemGroupName = "DuanEcomAdminSystem";
    public const string CatalogGroupName = "DuanEcomAdminCatalog";

    public static class Product
    {
        public const string Default = CatalogGroupName + ".Product";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string AttributeManage = Default + ".Attribute";

    }

    public static class Attribute
    {
        public const string Default = CatalogGroupName + ".Attribute";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    //Add your own permission names. Example:
    public static class Manufacturer
    {
        public const string Default = CatalogGroupName + ".Manufacturer";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ProductCategory
    {
        public const string Default = CatalogGroupName + ".ProductCategory";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}

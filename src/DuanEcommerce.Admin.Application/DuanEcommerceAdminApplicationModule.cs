using DuanEcommerce.Products;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace DuanEcommerce.Admin;

[DependsOn(
    typeof(DuanEcommerceDomainModule),
    typeof(DuanEcommerceAdminApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpBlobStoringFileSystemModule)
    )]
public class DuanEcommerceAdminApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<ProductThumbnailPictureContainer>(container =>
            {
                container.UseFileSystem(fileSystem =>
                {
                    // Thiết lập đường dẫn lưu trữ file vật lý trên đĩa (VD: wwwroot/uploads hoặc App_Data/uploads)
                    fileSystem.BasePath = "D:\\Abp.io\\Upload";
                });
            });
        });
    }
}

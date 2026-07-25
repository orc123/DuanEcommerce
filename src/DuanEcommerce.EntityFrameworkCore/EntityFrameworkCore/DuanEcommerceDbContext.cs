using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using DuanEcommerce.ProductAttributes;
using DuanEcommerce.Inventories;
using DuanEcommerce.InventoryTickets;
using DuanEcommerce.Manufacturers;
using DuanEcommerce.Orders;
using DuanEcommerce.ProductCategories;
using DuanEcommerce.Products;
using DuanEcommerce.Promotions;

namespace DuanEcommerce.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DuanEcommerceDbContext :
    AbpDbContext<DuanEcommerceDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // Ecommerce

    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTicket> InventoryTickets { get; set; }
    public DbSet<InventoryTicketItem> InventoryTicketItems { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderTransaction> OrderTransactions { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }


    #endregion

    public DuanEcommerceDbContext(DbContextOptions<DuanEcommerceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<Inventory>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Inventories");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.SKU)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.StockQuantity)
                .IsRequired();
        });


        builder.Entity<InventoryTicket>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "InventoryTickets");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();
        });

        builder.Entity<InventoryTicketItem>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "InventoryTicketItems");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.SKU)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.BatchNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        builder.Entity<Manufacturer>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Manufacturers");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.Slug)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.CoverPicture)
                .HasMaxLength(250);
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Orders");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.CustomerName)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.CustomerAddress)
                .HasMaxLength(250)
                .IsRequired();

            b.Property(x => x.CustomerPhoneNumber)
                .HasMaxLength(250)
                .IsRequired();
        });

        builder.Entity<OrderItem>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "OrderItems");
            b.ConfigureByConvention();
            b.HasKey(x => new { x.ProductId, x.OrderId});
            b.Property(x => x.SKU)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();
        });

        builder.Entity<OrderTransaction>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "OrderTransactions");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();
            b.Property(x => x.Note)
               .HasMaxLength(500);
        });

        builder.Entity<ProductAttribute>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttribute");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);

            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.Label)
               .HasMaxLength(50)
               .IsRequired();
        });

        builder.Entity<ProductCategory>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductCategories");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.Slug)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.CoverPicture)
                .HasMaxLength(250);

            b.Property(x => x.SeoMetaDescription)
                .HasMaxLength(250);
        });

        builder.Entity<ProductAttributeDateTime>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttributeDateTimes");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
        builder.Entity<ProductAttributeDecimal>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttributeDecimals");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
        builder.Entity<ProductAttributeInt>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttributeInts");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
        builder.Entity<ProductAttributeText>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttributeTexts");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Value)
                .HasMaxLength(500);
        });
        builder.Entity<ProductAttributeVarchar>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductAttributeVarchars");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Value)
               .HasMaxLength(500);
        });

        builder.Entity<Product>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Products");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.Slug)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.SKU)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            b.Property(x => x.ThumbnailPicture)
                .HasMaxLength(250);

            b.Property(x => x.SeoMetaDescription)
                .HasMaxLength(250);
        });

        builder.Entity<ProductLink>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductLinks");
            b.ConfigureByConvention();
            b.HasKey(x => new { x.ProductId, x.LinkedProductId});
        });

        builder.Entity<ProductReview>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductReviews");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();
        });

        builder.Entity<ProductTag>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "ProductTags");
            b.ConfigureByConvention();
            b.HasKey(x => new { x.ProductId, x.TagId });
        });
        builder.Entity<Tag>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Tags");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
               .HasMaxLength(50)
               .IsRequired();
            b.Property(x => x.Name)
              .HasMaxLength(50)
              .IsRequired();
        });

        builder.Entity<PromotionCategory>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "PromotionCategories");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });

        builder.Entity<Promotion>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "Promotions");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.CouponCode)
               .HasMaxLength(50)
               .IsUnicode(false)
               .IsRequired();
        });

        builder.Entity<PromotionManufacturer>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "PromotionManufacturers");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
        builder.Entity<PromotionProduct>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "PromotionProducts");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
        builder.Entity<PromotionUsageHistory>(b =>
        {
            b.ToTable(DuanEcommerceConsts.DbTablePrefix + "PromotionUsageHistories");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
        });
    }
}

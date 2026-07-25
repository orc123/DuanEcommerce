using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DuanEcommerce.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class DuanEcommerceDbContextFactory : IDesignTimeDbContextFactory<DuanEcommerceDbContext>
{
    public DuanEcommerceDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        DuanEcommerceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<DuanEcommerceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new DuanEcommerceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../DuanEcommerce.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}

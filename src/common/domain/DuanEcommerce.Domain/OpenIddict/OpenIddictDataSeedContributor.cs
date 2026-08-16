using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace DuanEcommerce.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor 
        {
            Name = "DuanEcommerce", 
            DisplayName = "DuanEcommerce API", 
            Resources = { "DuanEcommerce" }
        });

        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = "DuanEcommerce.Admin",
            DisplayName = "DuanEcommerce Admin API",
            Resources = { "DuanEcommerce" }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        var adminScopes = new List<string>();
        adminScopes.AddRange(commonScopes);
        adminScopes.Add("DuanEcommerce.Admin");

        var clientScopes = new List<string>();
        clientScopes.AddRange(commonScopes);
        clientScopes.Add("DuanEcommerce");

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");


        // Admin Client
        
        var appClientId = configurationSection["DuanEcommerce_Web:ClientId"];
        if (!appClientId.IsNullOrWhiteSpace())
        {
            var appClientRootUrl = configurationSection["DuanEcommerce_Web:RootUrl"]?.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: appClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Admin Application",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Implicit,
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.RefreshToken
                },
                scopes: adminScopes,
                redirectUris: new List<string> { appClientRootUrl },
                postLogoutRedirectUris: new List<string> { appClientRootUrl },
                clientUri: appClientRootUrl,
                logoUri: "/images/clients/angular.svg"
            );
        }


        // Swagger Client
        var swaggerClientId = configurationSection["DuanEcommerce_Admin:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["DuanEcommerce_Admin:RootUrl"]?.TrimEnd('/');

            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: swaggerClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger Application",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Implicit
                },
                scopes: adminScopes,
                redirectUris: new List<string> { $"{swaggerRootUrl}/swagger/oauth2-redirect.html" },
                clientUri: swaggerRootUrl.EnsureEndsWith('/') + "swagger",
                logoUri: "/images/clients/swagger.svg"
            );
        }

        // Web App
        var appWebClientId = configurationSection["DuanEcommerce_Web_Client:ClientId"];
        if (!appWebClientId.IsNullOrWhiteSpace())
        {
            var appClientRootUrl = configurationSection["DuanEcommerce_Web_Client:RootUrl"]?.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: appWebClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Client Application",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Implicit,
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.RefreshToken
                },
                scopes: clientScopes,
                redirectUris: new List<string> { appClientRootUrl, $"{appClientRootUrl}/signin-oidc" },
                postLogoutRedirectUris: new List<string> { appClientRootUrl, $"{appClientRootUrl}/signout-callback-oidc" },
                clientUri: appClientRootUrl,
                logoUri: "/images/clients/angular.svg"
            );
        }

    }
}

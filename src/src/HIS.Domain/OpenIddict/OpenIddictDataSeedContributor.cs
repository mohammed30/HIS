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

namespace HIS.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    private readonly IOpenIddictApplicationRepository _applicationRepository;

    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
        _applicationRepository = openIddictApplicationRepository;
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
            Name = "HIS", 
            DisplayName = "HIS API", 
            Resources = { "HIS" }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            "HIS"
        };

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");


        //Console Test / Angular Client - ONLY create if not exists to prevent overwriting RedirectUris
        var consoleAndAngularClientId = configurationSection["HIS_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            var existingApp = await _applicationRepository.FindByClientIdAsync(consoleAndAngularClientId);
            if (existingApp == null)
            {
                var consoleAndAngularClientRootUrl = configurationSection["HIS_App:RootUrl"]?.TrimEnd('/');
                await CreateOrUpdateApplicationAsync(
                    applicationType: OpenIddictConstants.ApplicationTypes.Web,
                    name: consoleAndAngularClientId!,
                    type: OpenIddictConstants.ClientTypes.Public,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Console Test / Angular Application",
                    secret: null,
                    grantTypes: new List<string> {
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.GrantTypes.Password,
                        OpenIddictConstants.GrantTypes.ClientCredentials,
                        OpenIddictConstants.GrantTypes.RefreshToken,
                        "LinkLogin",
                        "Impersonation"
                    },
                    scopes: commonScopes,
                    redirectUris: new List<string> { consoleAndAngularClientRootUrl },
                    postLogoutRedirectUris: new List<string> { consoleAndAngularClientRootUrl },
                    clientUri: consoleAndAngularClientRootUrl,
                    logoUri: "/images/clients/angular.svg"
                );
            }
        }

        
        




        // Swagger Client - ONLY create if not exists
        var swaggerClientId = configurationSection["HIS_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var existingSwagger = await _applicationRepository.FindByClientIdAsync(swaggerClientId);
            if (existingSwagger == null)
            {
                var swaggerRootUrl = configurationSection["HIS_Swagger:RootUrl"]?.TrimEnd('/');

                await CreateOrUpdateApplicationAsync(
                    applicationType: OpenIddictConstants.ApplicationTypes.Web,
                    name: swaggerClientId!,
                    type: OpenIddictConstants.ClientTypes.Public,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Swagger Application",
                    secret: null,
                    grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode, },
                    scopes: commonScopes,
                    redirectUris: new List<string> { $"{swaggerRootUrl}/swagger/oauth2-redirect.html" },
                    clientUri: swaggerRootUrl.EnsureEndsWith('/') + "swagger",
                    logoUri: "/images/clients/swagger.svg"
                );
            }
        }


    }
}

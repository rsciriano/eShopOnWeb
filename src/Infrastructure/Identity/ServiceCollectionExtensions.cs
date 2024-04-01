using AspNetCore.Identity.CosmosDb.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {        
        var databaseEngine = configuration.GetDatabaseEngine();

        IdentityBuilder identityBuilder = databaseEngine switch
        {
            DatabaseEngines.CosmosDb => services
                .AddCosmosIdentity<AppIdentityCosmosDbContext, ApplicationUser, IdentityRole, string>(cfg => { }),
            _ => services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()

        };

        identityBuilder
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();

        return services;
    }
}

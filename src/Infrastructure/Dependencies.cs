using System;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.eShopWeb.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services, bool isDevelopmentEnvironment)
    {
        var databaseEngine = configuration.GetDatabaseEngine();

        if (databaseEngine == DatabaseEngines.InMemory)
        {
            services.AddDbContext<CatalogContext>(c =>
               c.UseInMemoryDatabase("Catalog"));
         
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));
        }
        else if (databaseEngine == DatabaseEngines.SqlServer)
        {

            if (isDevelopmentEnvironment)
            {
                // use real database
                // Requires LocalDB which can be installed with SQL Server Express 2016
                // https://www.microsoft.com/en-us/download/details.aspx?id=54284
                services.AddDbContext<CatalogContext>(c =>
                    c.UseSqlServer(configuration.GetConnectionString("CatalogConnection")));

                // Add Identity DbContext
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
            }
            else
            {
                services.AddDbContext<CatalogContext>(c =>
                {
                    var connectionString = configuration[configuration["AZURE_SQL_CATALOG_CONNECTION_STRING_KEY"] ?? ""];
                    c.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
                });
                services.AddDbContext<AppIdentityDbContext>(options =>
                {
                    var connectionString = configuration[configuration["AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY"] ?? ""];
                    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
                });

            }

        }
        else if (databaseEngine == DatabaseEngines.CosmosDb)
        {

            var cosmosClientOptions = new CosmosClientOptions();
            ConfigureCosmosDb(cosmosClientOptions, isDevelopmentEnvironment);

            services.AddSingleton(new CosmosClient(configuration.GetConnectionString("CatalogConnection"), cosmosClientOptions));

            services.AddDbContext<CatalogContext>(c =>
                c.UseCosmos(
                    connectionString: configuration.GetConnectionString("CatalogConnection"),
                    databaseName: "Catalog",
                    cfg => ConfigureCosmosDb(cfg, isDevelopmentEnvironment)
                )
                .EnableSensitiveDataLogging(isDevelopmentEnvironment)
            );

            // Add Identity DbContext
            /*services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseCosmos(
                    connectionString: configuration.GetConnectionString("IdentityConnection"),
                    databaseName: "Identity",
                    cfg => ConfigureCosmosDb(cfg, isDevelopmentEnvironment))
                );*/


            services.AddDbContext<AppIdentityCosmosDbContext>(options =>
                options.UseCosmos(
                    connectionString: configuration.GetConnectionString("IdentityConnection"),
                    databaseName: "Identity",
                    cfg => ConfigureCosmosDb(cfg, isDevelopmentEnvironment))
                );
        }
        else
        {
            throw new NotSupportedException($"Database engine '{databaseEngine}' is not supported");
        }
    }

    private static void ConfigureCosmosDb(CosmosDbContextOptionsBuilder cfg, bool isDevelopmentEnvironment)
    {
        cfg.ConnectionMode(isDevelopmentEnvironment ? ConnectionMode.Gateway : ConnectionMode.Direct);
        if (isDevelopmentEnvironment)
        {
            cfg.HttpClientFactory(() => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }));
        }
    }

    private static void ConfigureCosmosDb(CosmosClientOptions cfg, bool isDevelopmentEnvironment)
    {
        cfg.ConnectionMode = isDevelopmentEnvironment ? ConnectionMode.Gateway : ConnectionMode.Direct;
        if (isDevelopmentEnvironment)
        {
            cfg.HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        }
    }

}

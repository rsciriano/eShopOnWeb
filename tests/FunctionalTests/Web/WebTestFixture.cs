using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.CosmosDb;
using Testcontainers.SqlEdge;
using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.Web;

public class TestApplication : WebApplicationFactory<IBasketViewModelService>, IAsyncLifetime
{
    private readonly string _environment = "Development";
    private SqlEdgeContainer _sqlEdgeContainer;
    private string _catalogConnectionString;
    private string _identityConnectionString;
    private string _databaseEngine = DatabaseEngines.CosmosDb;
    private CosmosDbContainer _cosmosDbContainer;

    public async Task InitializeAsync()
    {
        if (_databaseEngine == DatabaseEngines.SqlServer)
        {
            _sqlEdgeContainer = new SqlEdgeBuilder()
              .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
              .WithReuse(true)
          .Build();
            _sqlEdgeContainer.StartAsync().Wait();

            await CreateSqlServerDatabaseAsync("Catalog");

            _catalogConnectionString = new SqlConnectionStringBuilder(_sqlEdgeContainer.GetConnectionString())
            {
                InitialCatalog = "Catalog"
            }.ToString();

            _identityConnectionString = new SqlConnectionStringBuilder(_sqlEdgeContainer.GetConnectionString())
            {
                InitialCatalog = "Identity"
            }.ToString();
        }
        else if (_databaseEngine == DatabaseEngines.CosmosDb)
        {
            _cosmosDbContainer = new CosmosDbBuilder()
              .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
              .WithName("eShopOnWeb-FunctionalTests")
              .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE","127.0.0.1")
              .WithPortBinding(8081)
              .WithReuse(true)
              .Build();
            await _cosmosDbContainer.StartAsync();

            _catalogConnectionString = _cosmosDbContainer.GetConnectionString();
            _identityConnectionString = _cosmosDbContainer.GetConnectionString();
        }

    }

    protected override IHost CreateHost(IHostBuilder builder)
    {

        builder.UseEnvironment(_environment);

        builder.ConfigureHostConfiguration(webBuilder =>
        {
            webBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("DatabaseEngine", _databaseEngine),
                new KeyValuePair<string, string?>("ConnectionStrings:CatalogConnection", _catalogConnectionString),
                new KeyValuePair<string, string?>("ConnectionStrings:IdentityConnection", _identityConnectionString),
            });
        });
        /*
        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<CatalogContext>) ||
                d.ServiceType == typeof(DbContextOptions<AppIdentityDbContext>))
            .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddScoped(sp =>
            {
                return GetDbContextOptions<CatalogContext>(_databaseEngine, "Catalog");
                
            });
            services.AddScoped(sp =>
            {
                return GetDbContextOptions<AppIdentityDbContext>(_databaseEngine, "Identity");
            });
        });
        */

        return base.CreateHost(builder);
    }

    protected async Task CreateSqlServerDatabaseAsync(string databaseName)
    {
        using var connection = new SqlConnection(_sqlEdgeContainer.GetConnectionString());

        // TODO: Add your database migration here.
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE DATABASE " + databaseName;

        await connection.OpenAsync()
            .ConfigureAwait(false);

        await command.ExecuteNonQueryAsync()
            .ConfigureAwait(false);
    }

    protected DbContextOptions<TContext> GetDbContextOptions<TContext>(string databaseEngine, string databaseName)
        where TContext : DbContext
    {
        if (databaseEngine == DatabaseEngines.InMemory)
        {
            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return dbOptions;

        }
        else if (databaseEngine == DatabaseEngines.SqlServer)
        {
            var connectionString = new SqlConnectionStringBuilder(_sqlEdgeContainer.GetConnectionString());
            connectionString.InitialCatalog = databaseName;

            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(
                    connectionString: connectionString.ToString()
                )
                .Options;

            return dbOptions;

        }
        else if (databaseEngine == DatabaseEngines.CosmosDb)
        {
            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    connectionString: _cosmosDbContainer.GetConnectionString(),
                    databaseName: databaseName,
                    cfg => cfg
                        .HttpClientFactory(() => new HttpClient(new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }))
                        .ConnectionMode(Azure.Cosmos.ConnectionMode.Gateway)
                    )
                .Options;

            return dbOptions;

        }
        else
        {
            throw new NotSupportedException($"Invalid database engine '{databaseEngine}'");
        }


    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

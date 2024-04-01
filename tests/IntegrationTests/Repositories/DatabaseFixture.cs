using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using NSubstitute.Routing.Handlers;
using Testcontainers.CosmosDb;
using Testcontainers.SqlEdge;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories;
public class DatabaseFixture : IAsyncLifetime
{
    private DbContextOptions<CatalogContext> _dbOptions;
    private CosmosDbContainer _cosmosDbContainer;
    private SqlEdgeContainer _sqlEdgeContainer;

    private const string DEFAULT_DATABASE_NAME = "IntegrationTests"; 

    public async Task<TContext> CreateDbContextAsync<TContext>(string databaseEngine)
        where TContext : DbContext
    {
        if (databaseEngine == DatabaseEngines.InMemory)
        {
            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(databaseName: DEFAULT_DATABASE_NAME)
                .Options;

            return (TContext)Activator.CreateInstance(typeof(TContext), dbOptions);

        }
        else if (databaseEngine == DatabaseEngines.SqlServer)
        {
            _sqlEdgeContainer = new SqlEdgeBuilder()
              .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
              .WithName("eShopOnWeb-IntegrationTests")
              .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
              .WithReuse(true)
              .Build();
            await _sqlEdgeContainer.StartAsync();

            await CreateSqlServerDatabase(DEFAULT_DATABASE_NAME);

            var connectionString = new SqlConnectionStringBuilder(_sqlEdgeContainer.GetConnectionString());
            connectionString.InitialCatalog = DEFAULT_DATABASE_NAME;

            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(
                    connectionString: connectionString.ToString()
                )
                .Options;

            var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), dbOptions);
            await dbContext.Database.MigrateAsync();
            return dbContext;

        }
        else if (databaseEngine == DatabaseEngines.CosmosDb)
        {
            _cosmosDbContainer = new CosmosDbBuilder()
              .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
              .Build();
            await _cosmosDbContainer.StartAsync();

            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    connectionString: _cosmosDbContainer.GetConnectionString(), 
                    databaseName: DEFAULT_DATABASE_NAME,
                    cfg => cfg
                        .HttpClientFactory(() => new HttpClient(new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }))
                        .ConnectionMode(Azure.Cosmos.ConnectionMode.Gateway)
                    )
                .Options;

            return (TContext)Activator.CreateInstance(typeof(TContext), dbOptions);

        }
        else
        {
            throw new NotSupportedException($"Invalid database engine '{databaseEngine}'");
        }


    }

    protected async Task CreateSqlServerDatabase(string databaseName)
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

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_cosmosDbContainer is not null) await _cosmosDbContainer.DisposeAsync();
        if (_sqlEdgeContainer is not null) await _sqlEdgeContainer.DisposeAsync();

    }
}

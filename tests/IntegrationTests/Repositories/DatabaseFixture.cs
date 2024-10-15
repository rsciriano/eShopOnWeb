using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using Testcontainers.CosmosDb;
using Testcontainers.MsSql;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories;
public class DatabaseFixture : IAsyncLifetime
{
    private DbContextOptions<CatalogContext> _dbOptions;
    private CosmosDbContainer _cosmosDbContainer;
    private MsSqlContainer _sqlContainer;

    private const string DEFAULT_DATABASE_NAME = "IntegrationTests";

    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            // CosmosDB's preconfigured HTTP client will redirect the request to the container.
            const string requestUri = "https://localhost:/_explorer/Index.html";

            var httpClient = ((CosmosDbContainer)container).HttpClient;

            try
            {
                using var httpResponse = await httpClient.GetAsync(requestUri)
                    .ConfigureAwait(false);

                return httpResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CosmosDb: {ex.Message}");
                return false;
            }
            finally
            {
                httpClient.Dispose();
            }
        }
    }


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
            _sqlContainer = new MsSqlBuilder()
              .WithPortBinding(56300, MsSqlBuilder.MsSqlPort)
              .Build();
            await _sqlContainer.StartAsync();

            await CreateSqlServerDatabase(DEFAULT_DATABASE_NAME);

            var connectionString = new SqlConnectionStringBuilder(_sqlContainer.GetConnectionString());
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
              //.WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
              .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()))
              .Build();
            

            await _cosmosDbContainer.StartAsync();

            var dbOptions = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    connectionString: _cosmosDbContainer.GetConnectionString(), 
                    databaseName: DEFAULT_DATABASE_NAME,
                    cfg => cfg
                        /*.HttpClientFactory(() => new HttpClient(new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }))*/
                        .HttpClientFactory(() => _cosmosDbContainer.HttpClient)
                        .ConnectionMode(Azure.Cosmos.ConnectionMode.Gateway)
                    )
                .Options;

            var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), dbOptions);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;

        }
        else
        {
            throw new NotSupportedException($"Invalid database engine '{databaseEngine}'");
        }


    }

    protected async Task CreateSqlServerDatabase(string databaseName)
    {
        using var connection = new SqlConnection(_sqlContainer.GetConnectionString());

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
        if (_sqlContainer is not null) await _sqlContainer.DisposeAsync();

    }
}

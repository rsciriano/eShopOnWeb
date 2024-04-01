using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb;
public class AutoIncValueGenerator : ValueGenerator<int>
{
    public class Sequence
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public override bool GeneratesTemporaryValues { get => false; }

    public override int Next(EntityEntry entry)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<object?> NextValueAsync(EntityEntry entry, CancellationToken cancellationToken = default)
    {
        var sequenceName = entry.Metadata.ShortName();

        var cosmosClient = entry.Context.Database.GetCosmosClient();
        var database = cosmosClient.GetDatabase(entry.Context.Database.GetCosmosDatabaseId());
        await database.CreateContainerIfNotExistsAsync(nameof(AutoIncValueGenerator), "/id");
        var container = database.GetContainer(nameof(AutoIncValueGenerator));

        var operations = new[]
        {
            PatchOperation.Increment("/value", 1)
        };

        try
        { 
            var response = await container.PatchItemAsync<Sequence>(
                sequenceName, 
                new PartitionKey(sequenceName), 
                operations,
                cancellationToken: cancellationToken);
            return response.Resource.Value;
        }
        catch (CosmosException ex) 
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var response = await container.CreateItemAsync<Sequence>(
                    new Sequence { Id = sequenceName, Value = 1 }, 
                    partitionKey: new PartitionKey(sequenceName),
                    cancellationToken: cancellationToken);
                return 1;
            }
            throw;
        }
    }

    protected override object? NextValue(EntityEntry entry)
    {
        throw new NotImplementedException();
    }
}

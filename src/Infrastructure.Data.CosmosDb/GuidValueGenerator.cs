using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb;
public class GuidValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues { get => false; }

    public override string Next(EntityEntry entry)
    {
        return Guid.NewGuid().ToString();
    }
}

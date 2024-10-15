using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb;
public class GuidValueGenerator : ValueGenerator<Guid>
{
    public override bool GeneratesTemporaryValues { get => false; }

    public override Guid Next(EntityEntry entry)
    {
        return Guid.NewGuid();
    }
}

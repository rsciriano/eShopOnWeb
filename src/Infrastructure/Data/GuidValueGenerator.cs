using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.eShopWeb.Infrastructure.Data;
public class GuidValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues { get => false; }

    public override string Next(EntityEntry entry)
    {
        return Guid.NewGuid().ToString();
    }
}

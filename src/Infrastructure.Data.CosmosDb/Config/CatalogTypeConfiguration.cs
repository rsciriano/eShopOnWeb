using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb.Config;

public class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToContainer("CatalogTypes");
        builder.HasPartitionKey(e => e.Id);

        builder.HasNoDiscriminator();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ToJsonProperty("id")
            .HasConversion<string>()
            .HasValueGenerator<AutoIncValueGenerator>();
    }
}

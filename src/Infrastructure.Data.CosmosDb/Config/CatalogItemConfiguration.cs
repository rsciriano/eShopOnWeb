using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb.Config;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToContainer("CatalogItems");
        builder.HasPartitionKey(e => e.Id);

        builder.HasNoDiscriminator();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ToJsonProperty("id")
            .HasConversion<string>()
            .HasValueGenerator<AutoIncValueGenerator>();
    }
}

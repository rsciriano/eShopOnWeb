using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb.Config;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.ToContainer("Baskets");
        //builder.HasPartitionKey(e => e.Id);
        builder.HasPartitionKey(e => e.BuyerId);

        builder.HasNoDiscriminator();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ToJsonProperty("id")
            .HasConversion<string>()
            //.HasValueGenerator<AutoIncValueGenerator>();
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(e => e.BuyerId)
            .HasConversion<string>();

        builder.OwnsMany(e => e.Items)
            .HasKey(new[] { nameof(BasketItem.BasketId), nameof(BasketItem.CatalogItemId) });
            
    }
}

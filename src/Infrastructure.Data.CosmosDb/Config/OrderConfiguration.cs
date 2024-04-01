using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToContainer("Orders");
        //builder.HasPartitionKey(e => e.Id);
        builder.HasPartitionKey(e => e.BuyerId);

        builder.HasNoDiscriminator();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ToJsonProperty("id")
            .HasConversion<string>()
            //.HasValueGenerator<AutoIncValueGenerator>();
            .HasValueGenerator<GuidValueGenerator>();

        builder.OwnsMany(e => e.OrderItems);
    }
}

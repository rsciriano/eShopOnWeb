using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable("BasketItems");

        builder.Property<int>("Id");

        builder.Property(bi => bi.UnitPrice)
            .IsRequired(true)
            .HasColumnType("decimal(18,2)");
    }
}

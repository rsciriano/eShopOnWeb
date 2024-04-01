using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.Infrastructure.Data.CosmosDb.Config;

public class CatalogBrandConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToContainer("CatalogBrands");
        builder.HasPartitionKey(e => e.Id);
        
        builder.HasNoDiscriminator();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ToJsonProperty("id")
            .HasConversion<string>()
            .HasValueGenerator<AutoIncValueGenerator>();


        /*
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
           .UseHiLo("catalog_brand_hilo")
           .IsRequired();

        builder.Property(cb => cb.Brand)
            .IsRequired()
            .HasMaxLength(100);
        */
    }
}

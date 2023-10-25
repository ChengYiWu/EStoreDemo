using Domain.Product;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> productItemConfiguration)
    {
        productItemConfiguration.ToTable("ProductItem");

        productItemConfiguration.HasKey(x => x.Id);


        productItemConfiguration.Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();

        productItemConfiguration.Property(x => x.Price)
            .HasColumnType("decimal(7,2)")
            .IsRequired();

        productItemConfiguration.Property(x => x.StockQuantity)
            .IsRequired();

        productItemConfiguration.Property(x => x.IsActive)
            .HasDefaultValue(false);

        productItemConfiguration
            .HasOne(p => p.Image)
            .WithOne(p => p.ProductItem)
            .OnDelete(DeleteBehavior.NoAction);

        productItemConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

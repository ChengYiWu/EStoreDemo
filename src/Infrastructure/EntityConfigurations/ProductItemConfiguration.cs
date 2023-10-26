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

        // 聯集刪除關係請讀這篇文章:https://learn.microsoft.com/zh-tw/ef/core/saving/cascade-delete
        productItemConfiguration
            .HasOne(p => p.Image)
            .WithOne(p => p.ProductItem)
            .OnDelete(DeleteBehavior.ClientCascade);

        productItemConfiguration.Property(x => x.CreatedAt)
            .IsRequired();

        productItemConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.ClientCascade);

        productItemConfiguration.Property(x => x.UpdatedAt);

        productItemConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

using Domain.Product;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> productConfiguration)
    {
        productConfiguration.ToTable("Product");

        productConfiguration.HasKey(x => x.Id);

        productConfiguration.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        productConfiguration.Property(x => x.Description)
            .IsRequired();

        productConfiguration.Property(x => x.Brand)
            .HasMaxLength(64);

        productConfiguration.Property(x => x.Weight)
            .HasMaxLength(32);

        productConfiguration.Property(x => x.Dimensions)
            .HasMaxLength(32);

        productConfiguration.HasMany(x => x.ProductItems);

        // 聯集刪除關係請讀這篇文章:https://learn.microsoft.com/zh-tw/ef/core/saving/cascade-delete
        productConfiguration.HasMany(x => x.Images)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.ClientCascade);

        productConfiguration.Property(x => x.CreatedAt)
            .IsRequired();

        productConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.ClientCascade);

        productConfiguration.Property(x => x.UpdatedAt);

        productConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

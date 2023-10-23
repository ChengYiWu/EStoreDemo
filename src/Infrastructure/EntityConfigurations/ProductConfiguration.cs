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

        productConfiguration.HasMany(p => p.Images);

        productConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

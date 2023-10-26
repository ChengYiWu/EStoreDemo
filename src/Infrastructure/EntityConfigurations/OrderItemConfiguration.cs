using Domain.Order;
using Domain.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
    {
        orderItemConfiguration.ToTable("OrderItem");

        orderItemConfiguration.HasKey(x => x.Id);

        orderItemConfiguration.Property(x => x.Price)
            .HasColumnType("decimal(7,2)")
            .IsRequired();

        orderItemConfiguration.Property(x => x.Quantity)
            .IsRequired();

        orderItemConfiguration.HasOne<ProductItem>()
            .WithMany()
            .HasForeignKey(x => x.ProductItemId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

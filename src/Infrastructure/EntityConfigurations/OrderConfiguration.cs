using Domain.Order;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.ToTable("Order");

        orderConfiguration.HasKey(x => x.Id);

        orderConfiguration.HasIndex(x => x.OrderNo)
            .IsUnique();

        orderConfiguration.Property(x => x.OrderNo)
            .HasMaxLength(64)
            .IsRequired();

        orderConfiguration.Property(x => x.Note)
            .HasMaxLength(512);

        orderConfiguration.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        orderConfiguration.Property(x => x.IsEditable)
           .IsRequired()
           .HasDefaultValue(true);

        orderConfiguration.OwnsOne(x => x.ShippingInfo, shippingInfo =>
        {
            shippingInfo.Property(p => p.ContactPhone)
                .HasColumnName("ContactPhone")
                .HasMaxLength(32)
                .IsRequired();

            shippingInfo.Property(p => p.ShippingAddress)
                .HasColumnName("ShippingAddress")
                .HasMaxLength(1024)
                .IsRequired();

            shippingInfo.Property(p => p.ShippingFee)
                .HasColumnName("ShippingFee")
                .HasColumnType("decimal(7,2)")
                .IsRequired();

            shippingInfo.Property(p => p.ShippedBy)
                .HasColumnName("ShippedBy")
                .IsRequired(false);

            shippingInfo.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.ShippedBy)
                .OnDelete(DeleteBehavior.ClientCascade);

            shippingInfo.Property(p => p.ShippedAt)
                .HasColumnName("ShippedAt")
                .IsRequired(false);
        });

        orderConfiguration.Property(x => x.TotalPrice)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        orderConfiguration.Property(x => x.PlacedAt)
            .IsRequired();

        orderConfiguration.Property(x => x.CancelledReason)
            .HasMaxLength(512)
            .IsRequired(false);

        orderConfiguration.Property(x => x.FinalTotalPrice)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        orderConfiguration.Property(x => x.PriceDiscount)
            .HasColumnType("decimal(10,2)");

        orderConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CancelledBy)
            .OnDelete(DeleteBehavior.ClientCascade);

        orderConfiguration.Property(x => x.CancelledAt)
            .IsRequired(false);

        orderConfiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.ClientCascade);

        orderConfiguration.HasOne(x => x.UsedCoupon)
            .WithMany()
            .HasForeignKey(x => x.UsedCouponId)
            .OnDelete(DeleteBehavior.ClientCascade);

        orderConfiguration.HasMany(x => x.OrderItems);
    }
}

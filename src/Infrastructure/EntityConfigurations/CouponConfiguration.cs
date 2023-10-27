using Azure;
using Domain.Coupon;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> couponCondiguration)
    {
        couponCondiguration.ToTable("Coupon");

        couponCondiguration.HasKey(p => p.Id);

        couponCondiguration.Property(p => p.Title)
            .HasMaxLength(128)
            .IsRequired();

        couponCondiguration.Property(p => p.Code)
            .HasMaxLength(64)
            .IsRequired();

        couponCondiguration.Property(p => p.Description)
            .IsRequired();

        couponCondiguration.Property(x => x.IsActive)
           .HasDefaultValue(false);

        couponCondiguration.HasDiscriminator(x => x.Type)
            .HasValue<PriceAmountDiscountCoupon>(CouponType.PriceAmountDiscount)
            .HasValue<PricePercentDiscountCoupon>(CouponType.PricePercentDiscount);

        couponCondiguration.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(128);

        couponCondiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.ClientCascade);

        couponCondiguration.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.UpadtedBy)
            .OnDelete(DeleteBehavior.ClientCascade);

        couponCondiguration.HasMany(x => x.ApplicableProducts);
    }
}

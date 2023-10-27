using Domain.Coupon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class PriceAmountDiscountCouponConfiguration : IEntityTypeConfiguration<PriceAmountDiscountCoupon>
{
    public void Configure(EntityTypeBuilder<PriceAmountDiscountCoupon> config)
    {
        config.Property(x => x.PriceAmountDiscount)
            .HasColumnType("decimal(7,2)")
            .IsRequired();
    }
}

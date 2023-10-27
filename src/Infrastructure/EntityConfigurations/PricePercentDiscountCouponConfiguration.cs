using Domain.Coupon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class PricePercentDiscountCouponConfiguration : IEntityTypeConfiguration<PricePercentDiscountCoupon>
{
    public void Configure(EntityTypeBuilder<PricePercentDiscountCoupon> config)
    {
        config.Property(x => x.PricePercentDiscount)
            .HasColumnType("decimal(4,2)")
            .IsRequired();
    }
}

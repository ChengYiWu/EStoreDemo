using Domain.Coupon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class CouponApplicableProductConfiguration : IEntityTypeConfiguration<CouponApplicableProduct>
{
    public void Configure(EntityTypeBuilder<CouponApplicableProduct> config)
    {
        config.ToTable("CouponApplicableProduct")
            .HasKey(x => new { x.CouponId, x.ProductId });

        config.HasOne(x => x.Coupon)
            .WithMany(x => x.ApplicableProducts)
            .HasForeignKey(x => x.CouponId);

        config.HasOne(x => x.Product)
            .WithMany(x => x.Coupons)
            .HasForeignKey(x => x.ProductId);
    }
}


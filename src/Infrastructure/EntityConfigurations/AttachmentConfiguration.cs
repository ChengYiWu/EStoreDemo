using Domain.Attachment;
using Domain.Product;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> attachmentConfiguartion)
    {
        attachmentConfiguartion.ToTable("Attachment");

        attachmentConfiguartion.HasKey(p => p.Id);

        // 需注意，採用 TPH 策略時，是無法將已存在的資料轉換成其他子類別，只能刪除原有資料後，重新新增
        // 因此，如果有需要將資料轉換成其他子類別的需求，請採用 TPT 策略
        attachmentConfiguartion.HasDiscriminator<string>("ContentType")
            .HasValue<Attachment>("Attachment")
            .HasValue<ProductImageAttachment>("ProductImageAttachment")
            .HasValue<ProductItemImageAttachment>("ProductItemImageAttachment");

        attachmentConfiguartion.Property(p => p.OriFileName)
            .HasMaxLength(256)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.FileName)
            .HasMaxLength(256)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.Path)
            .HasMaxLength(1024)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.Uri)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.CreatedAt)
            .IsRequired();

        attachmentConfiguartion.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

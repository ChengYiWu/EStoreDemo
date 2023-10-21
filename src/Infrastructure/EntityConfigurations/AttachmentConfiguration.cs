using Domain.Attachment;
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

        attachmentConfiguartion.Property(p => p.Type)
            .HasMaxLength(256);

        attachmentConfiguartion.HasDiscriminator<string>("Type")
            .HasValue<Attachment>("Attachment");

        attachmentConfiguartion.Property(p => p.OriFileName)
            .HasMaxLength(256)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.FileName)
            .HasMaxLength(256)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.Path)
            .HasMaxLength(1024)
            .IsRequired();

        attachmentConfiguartion.Property(p => p.CreatedAt)
            .IsRequired();

        attachmentConfiguartion.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

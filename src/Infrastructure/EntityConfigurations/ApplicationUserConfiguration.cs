using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> applicationUserConfiguration)
    {
        // 可在此處覆寫預設的 Table 設定，可將不必要的欄位移除，如 ：PhoneNumber、EmailConfirmed ... 等

        applicationUserConfiguration.ToTable("User");

        applicationUserConfiguration.Property(p => p.UserName)
            .IsRequired();

        applicationUserConfiguration.Property(p => p.Email)
            .IsRequired();

        applicationUserConfiguration.Ignore(p => p.Roles);
    }
}

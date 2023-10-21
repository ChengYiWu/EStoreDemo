using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;

namespace Infrastructure.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> roleConfiguration)
    {
        roleConfiguration.ToTable("Role");

        roleConfiguration.HasIndex(p => p.Name)
            .IsUnique();

        roleConfiguration.Property(p => p.Name)
            .IsRequired();
    }
}


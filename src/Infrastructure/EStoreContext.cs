using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure;

public class EStoreContext : IdentityDbContext<ApplicationUser>
{
    public EStoreContext(DbContextOptions<EStoreContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 設定預設字串排序規則
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

        // 使 IdentityDbContext 可以執行預設的 Identity Table 設定
        base.OnModelCreating(modelBuilder);

        // 載入所有實作 IEntityTypeConfiguration<T> 的類別，自動套用其設定
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        IgnoreUnUsedIdentityEntities(modelBuilder);
    }

    /// <summary>
    /// 避免產生一些目前尚不需使用的 Identity Table
    /// </summary>
    /// <param name="modelBuilder"></param>
    private void IgnoreUnUsedIdentityEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<IdentityUserClaim<string>>();
        modelBuilder.Ignore<IdentityUserLogin<string>>();
        modelBuilder.Ignore<IdentityRoleClaim<string>>();
        modelBuilder.Ignore<IdentityUserToken<string>>();
    }
}
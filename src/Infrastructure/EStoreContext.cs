using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure;

public class EStoreContext : DbContext
{
    public EStoreContext(DbContextOptions<EStoreContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 設定預設字串排序規則
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

        // 載入所有實作 IEntityTypeConfiguration<T> 的類別，自動套用其設定
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Infrastructure.Options;
using Application.Common.Identity;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;

namespace Infrastructure;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<EStoreContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

/// <summary>
/// 初始化資料庫
/// </summary>
public class EStoreContextInitialiser
{
    private readonly EStoreContext _context;

    private readonly ILogger<EStoreContextInitialiser> _logger;

    private readonly SeedDataOption _seedDataOption;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<ApplicationRole> _roleManager;

    public EStoreContextInitialiser(
        EStoreContext context,
        ILogger<EStoreContextInitialiser> logger,
        IOptions<SeedDataOption> seedDataOption,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
        )
    {
        _context = context;
        _logger = logger;
        _seedDataOption = seedDataOption.Value;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// 執行 EF Core Code First Migration
    /// </summary>
    /// <returns></returns>
    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行資料庫 Migration 時發生錯誤。");
            throw;
        }
    }

    /// <summary>
    /// 執行預設資料寫入作業
    /// </summary>
    /// <returns></returns>
    public async Task SeedAsync()
    {
        try
        {
            // 建立預設 Role 資料
            var roles = Enum.GetNames(typeof(RoleEnum));
            var existedRoles = await _roleManager.Roles.ToListAsync();

            foreach (string role in roles)
            {
                if (!existedRoles.Any(r => r.Name == role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole(role));
                }
            }

            // 建立預設系統管理員使用者資料
            var adminEmail = _seedDataOption.Admin.Email;
            var adminPassword = _seedDataOption.Admin.Password;

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                };

                var result = await _userManager.CreateAsync(newUser, adminPassword);

                if (!result.Succeeded)
                {
                    _logger.LogError($"建立預設 Admin 使用者失敗（{result}）。");
                    return;
                }

                await _userManager.AddToRolesAsync(newUser, roles);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行資料庫 Seed 資料寫入時發生錯誤。");
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

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
    private readonly ILogger<EStoreContextInitialiser> _logger;
    private readonly EStoreContext _context;

    public EStoreContextInitialiser(ILogger<EStoreContextInitialiser> logger, EStoreContext context)
    {
        _logger = logger;
        _context = context;
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
    public Task SeedAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行資料庫 Seed 資料寫入時發生錯誤。");
            throw;
        }

        return Task.CompletedTask;
    }
}

using Application.Common.Identity;
using Application.Common.Interfaces;
using Application.Common.Services.FileService;
using Domain.Common;
using Domain.Coupon;
using Domain.Order;
using Domain.Product;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.FileService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastuctureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeedDataOption>(configuration.GetSection("SeedData"));
        services.Configure<JwtSettingsOption>(configuration.GetSection("JwtSettings"));
        services.Configure<ConnectionStringOption>(configuration.GetSection("ConnectionStrings"));
        services.Configure<FileStorageOption>(configuration.GetSection("BlobStorage"));

        // DbContext
        var connectionStringOptions = configuration.GetSection("ConnectionStrings").Get<ConnectionStringOption>();
        services.AddDbContext<EStoreContext>(
            c => c.UseSqlServer(connectionStringOptions.DefaultConnection, options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(15), errorNumbersToAdd: null);
            }),
            ServiceLifetime.Scoped
        );

        // Identity
        services
         .AddIdentityCore<ApplicationUser>()
         .AddRoles<ApplicationRole>()
         .AddEntityFrameworkStores<EStoreContext>();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // User settings.
            options.User.RequireUniqueEmail = true;
        });

        services.AddScoped<EStoreContextInitialiser>();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddSingleton<IProductFileUploadService, ProductFileUploadService>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();

        return services;
    }
}

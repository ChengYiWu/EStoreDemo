using Application.Common.Identity;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Options;
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

        // DbContext
        var connectionStringOptions = configuration.GetSection("ConnectionStrings").Get<ConnectionStringOption>();
        services.AddDbContext<EStoreContext>(
            c => c.UseSqlServer(connectionStringOptions.DefaultConnection),
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

        return services;
    }
}

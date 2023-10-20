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
        // DbContext
        var connectionStringOptions = configuration.GetSection("ConnectionStrings").Get<ConnectionStringOption>();
        services.AddDbContext<EStoreContext>(
            c => c.UseSqlServer(connectionStringOptions.DefaultConnection),
            ServiceLifetime.Scoped
        );
        services.AddScoped<EStoreContextInitialiser>();

        // Identity
        services
         .AddIdentityCore<ApplicationUser>()
         .AddRoles<IdentityRole<string>>()
         .AddEntityFrameworkStores<EStoreContext>();

        return services;
    }
}

using Infrastructure;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastuctureServices(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionStringOptions = configuration.GetSection("ConnectionStrings").Get<ConnectionStringOption>();

        services.AddDbContext<EStoreContext>(
            c => c.UseSqlServer(connectionStringOptions.DefaultConnection),
            ServiceLifetime.Scoped
        );
        services.AddScoped<EStoreContextInitialiser>();

        return services;
    }
}

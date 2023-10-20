using Application.Common.Behaviours;
using FluentValidation;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 載入所有 FluentValidation 的驗證器
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 註冊 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBecaviour<,>));
        });

        return services;
    }
}

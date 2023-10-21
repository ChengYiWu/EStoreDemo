using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using WebAPI.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependentInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettingsOption? jwtSettingOption = configuration
            .GetSection("JwtSettings")
            .Get<JwtSettingsOption>() 
            ?? throw new ArgumentException(nameof(JwtSettingsOption));

        // JWT Token 驗證設定
        var issuer = jwtSettingOption.Issuer;
        var secret = jwtSettingOption.Secret;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 透過這項宣告，就可以從 "NAME" 取值
                    NameClaimType = ClaimTypes.NameIdentifier,
                    // 透過這項宣告，就可以從 "Role" 取值，並可讓 [Authorize] 判斷角色
                    RoleClaimType = ClaimTypes.Role,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
            });

        // Swagger 設定
        services.AddSwaggerGen(options =>
        {
            // 加入驗證定義（如: bearer），介面才可產生驗證按鈕
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.OperationFilter<SwaggerAuthOperationsFilter>();
        });

        return services;
    }
}

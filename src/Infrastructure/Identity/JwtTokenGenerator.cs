using Application.Common.Identity;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity;

/// <summary>
/// 產生 JWT Token
/// </summary>
internal class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettingsOption _jwtSettingOption;

    public JwtTokenGenerator(IOptions<JwtSettingsOption> jwtSettingOption)
    {
        _jwtSettingOption = jwtSettingOption.Value;
    }

    /// <summary>
    /// 產生使用者的 token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="expiredMinutes">Token 過期時間</param>
    /// <returns>使用者 token</returns>
    public string GenerateToken(IUser user, int expiredMinutes = 30)
    {
        var issuer = _jwtSettingOption.Issuer;
        var secret = _jwtSettingOption.Secret;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow
                        .AddMinutes(expiredMinutes)
                        .ToUnixTimeSeconds()
                        .ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // add role claims
        if (user.Roles.Any())
        {
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        }

        var userClaimsIdentity = new ClaimsIdentity(claims);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
            //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
            //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expiredMinutes),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }
}

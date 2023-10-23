using Application.Common.Identity;
using System.Security.Claims;

namespace Infrastructure.Identity;

public interface ITokenGenerator
{
    /// <summary>
    /// 產生使用者的 token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="roles">使用者所屬角色</param>
    /// <param name="expiredMinutes">Token 過期時間</param>
    /// <returns>使用者 token</returns>
    string GenerateToken(IUser user, int expiredMinutes = 30);

    /// <summary>
    /// 從 ClaimsPrincipal 取得使用者 Id
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    string GetUserId(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// 從 ClaimsPrincipal 取得使用者
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    IUser GetUser(ClaimsPrincipal claimsPrincipal);
}

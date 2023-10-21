using Application.Common.Identity;

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
}

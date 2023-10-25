namespace Application.Common.Identity;

/// <summary>
/// 使用者識別與驗證權限相關服務
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// 建立使用者
    /// </summary>
    /// <param name="email">電子信箱</param>
    /// <param name="userName">使用者名稱</param>
    /// <param name="password">使用者密碼</param>
    /// <param name="roleNames">使用者角色名稱</param></param>
    /// <returns>使用者 Id</returns>
    Task<string> CreateUserAsync(string email, string userName, string password, string[]? roleNames);

    /// <summary>
    /// 修改使用者
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <param name="roleNames">使用者角色名稱</param>
    /// <returns></returns>
    Task<bool> UpdateUserAsync(IUser user, string[]? roleNames);

    /// <summary>
    /// 驗證使用者密碼
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="password">使用者密碼</param>
    /// <returns>密碼是否正確</returns>
    Task<bool> CheckUserPasswordAsync(IUser user, string password);

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <param name="oldPassword">舊密碼</param>
    /// <param name="newPassword">新密碼</param>
    /// <returns></returns>
    Task<bool> UpdatePasswordAsync(IUser user, string oldPassword, string newPassword);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <returns></returns>
    Task<bool> DeleteUserAsync(IUser user);

    /// <summary>
    /// 依據使用者信箱取得使用者
    /// </summary>
    /// <param name="email">使用者信箱</param>
    /// <returns>使用者</returns>
    Task<IUser?> GetUserByEmailAsync(string email);

    /// <summary>
    /// 依據使用者 Id 取得使用者
    /// </summary>
    /// <param name="id">使用者 Id</param>
    /// <returns></returns>
    Task<IUser?> GetUserByIdAsync(string id);

    /// <summary>
    /// 產生使用者 Token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <returns>使用者 Token</returns>
    string GenerateUserToken(IUser user);
}

using Application.Common.Exceptions;
using Application.Common.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator, ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    /// <summary>
    /// 建立使用者
    /// </summary>
    /// <param name="email">電子信箱</param>
    /// <param name="userName">使用者名稱</param>
    /// <param name="password">使用者密碼</param>
    /// <param name="roleNames">使用者角色</param>
    /// <returns>使用者 Id</returns>
    public async Task<string> CreateUserAsync(string email, string userName, string password, string[]? roleNames)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };

        var createUserResult = await _userManager.CreateAsync(user, password);

        if (!createUserResult.Succeeded)
        {
            throw new InternalException($"建立使用者失敗（{email}）。");
        }

        if (roleNames is not null && roleNames.Any())
        {
            var createUserRoleResult = await _userManager.AddToRolesAsync(user, roleNames);

            if (!createUserRoleResult.Succeeded)
            {
                throw new InternalException($"建立使用者角色失敗（{email}）。");
            }
        }


        return user.Id;
    }

    /// <summary>
    /// 驗證使用者密碼
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="password">使用者密碼</param>
    /// <returns>密碼是否正確</returns>
    public async Task<bool> CheckUserPasswordAsync(IUser user, string password)
    {
        if (user is ApplicationUser appUser)
        {
            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        throw new InternalException("使用者物件類型不允許。");
    }

    /// <summary>
    /// 依據使用者信箱取得使用者
    /// </summary>
    /// <param name="email">使用者信箱</param>
    /// <returns>使用者</returns>
    public async Task<IUser?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) return null;

        var roleNames = await _userManager.GetRolesAsync(user);

        user.Roles = roleNames.Select(roleName => new ApplicationRole(roleName)).ToList();

        return user;
    }

    /// <summary>
    /// 依據使用者 Id 取得使用者
    /// </summary>
    /// <param name="id">使用者 Id</param>
    /// <returns></returns>
    public async Task<IUser?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null) return null;

        var roleNames = await _userManager.GetRolesAsync(user);

        user.Roles = roleNames.Select(roleName => new ApplicationRole(roleName)).ToList();

        return user;
    }

    /// <summary>
    /// 產生使用者 Token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <returns>使用者 Token</returns>
    public string GenerateUserToken(IUser user)
    {
        // TODO: 先用一個長一點的時間，之後再改為短時間
        return _tokenGenerator.GenerateToken(user, 525600);
    }

    /// <summary>
    /// 修改使用者
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <param name="roleNames">使用者角色名稱</param>
    /// <returns></returns>
    public async Task<bool> UpdateUserAsync(IUser user, string[]? roleNames)
    {
        var updateUserResult = await _userManager.UpdateAsync((ApplicationUser)user);

        if (!updateUserResult.Succeeded)
        {
            _logger.LogError("更新使用者資訊失敗（{0}，{1}）", user.Id, updateUserResult.ToString());
            throw new InternalException("更新使用者資訊失敗");
        }

        var oriRoles = user.Roles.Select(role => role.Name).ToArray();

        var addRoles = roleNames?.Except(oriRoles).ToArray() ?? Array.Empty<string>();
        if (addRoles.Any())
        {
            await _userManager.AddToRolesAsync((ApplicationUser)user, addRoles);
        }

        var deleteRoles = oriRoles.Except(roleNames ?? Array.Empty<string>()).ToArray();
        if (deleteRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync((ApplicationUser)user, deleteRoles);
        }

        return true;
    }

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <param name="newPassword">新密碼</param>
    /// <returns></returns>
    public async Task<bool> UpdatePasswordAsync(IUser user, string oldPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync((ApplicationUser)user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            _logger.LogError("更新使用者密碼資訊失敗（{ 0}，{ 1}）。", user.Id, result.ToString());
            throw new InternalException("更新使用者密碼資訊失敗。");
        }

        return true;
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    /// <param name="user">使用者資訊</param>
    /// <returns></returns>
    public async Task<bool> DeleteUserAsync(IUser user)
    {
        var result = await _userManager.DeleteAsync((ApplicationUser)user);

        if (!result.Succeeded)
        {
            _logger.LogError("刪除使用者資訊失敗（{ 0}，{ 1}）。", user.Id, result.ToString());
            throw new InternalException("刪除使用者資訊失敗。");
        }

        return true;
    }
}

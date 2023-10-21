using Application.Common.Exceptions;
using Application.Common.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ITokenGenerator _tokenGenerator;

    public IdentityService(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
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
    /// 產生使用者 Token
    /// </summary>
    /// <param name="user">使用者</param>
    /// <returns>使用者 Token</returns>
    public string GenerateUserToken(IUser user)
    {
        return _tokenGenerator.GenerateToken(user);
    }
}

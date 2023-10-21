namespace Application.Auths.Commands.LoginUser;

public class LoginResponse
{
    /// <summary>
    /// 登入成功 token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 登入成功 refresh token
    /// </summary>
    public string RefreshToken { get; set; }
}


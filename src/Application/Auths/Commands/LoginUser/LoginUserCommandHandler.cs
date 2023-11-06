using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Auths.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.Email);

        // 避免回傳太精確錯誤訊息，以免被惡意使用者利用
        if (user is null || !await _identityService.CheckUserPasswordAsync(user, request.Password))
        {
            throw new FailureException("使用者不存在或密碼錯誤。");
        }

        LoginResponse response = new()
        {
            User = user,
            Token = _identityService.GenerateUserToken(user),
            // [ TODO ] 尚未實作 refresh token 機制
            // 機制：理論上須搭配 Redis 黑名單與資料庫紀錄 refresh token，並設定 token 為短期有效
            RefreshToken = string.Empty,
        };

        return response;
    }
}

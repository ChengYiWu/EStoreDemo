using Application.Common.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebAPI.Infrastructure;

public class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenGenerator _tokenGenerator;

    public HttpContextCurrentUser(
        IHttpContextAccessor httpContextAccessor,
        ITokenGenerator tokenGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenGenerator = tokenGenerator;
    }

    public IUser GetCurrentUser()
    {
        return _tokenGenerator.GetUser(GetClaimsPrincipalIfEmptyThrow());
    }

    public string GetCurrentUserId()
    {
        return _tokenGenerator.GetUserId(GetClaimsPrincipalIfEmptyThrow());
    }

    private ClaimsPrincipal GetClaimsPrincipalIfEmptyThrow()
    {
        ClaimsPrincipal? claimsPrincipal = _httpContextAccessor.HttpContext?.User;

        return claimsPrincipal is not null
            ? claimsPrincipal
            : throw new Exception("請求未包含使用者資訊。");
    }
}

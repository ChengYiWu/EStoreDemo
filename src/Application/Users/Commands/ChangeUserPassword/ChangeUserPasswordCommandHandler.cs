using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, bool>
{
    private readonly IIdentityService _identityService;

    public ChangeUserPasswordHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"找不到使用者（{request.Id}）。");
        }

        var isValidOldPassword = await _identityService.CheckUserPasswordAsync(user, request.OldPassword);

        if (!isValidOldPassword)
        {
            throw new FailureException("舊密碼不正確。");
        }

        return await _identityService.UpdatePasswordAsync(user, request.OldPassword, request.NewPassword);
    }
}

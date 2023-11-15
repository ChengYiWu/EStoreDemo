using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;

    public UpdateUserCommandHandler(IIdentityService identityService, ICurrentUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"找不到使用者（{request.Id}）。");
        }

        var currentUser = _currentUser.GetCurrentUser();

        if (!currentUser.Roles.Any(r => r.Name == RoleEnum.Admin.ToString()))
        {
            throw new FailureException("權限不足，無法執行此操作。");
        }

        user.UserName = request.UserName;

        return await _identityService.UpdateUserAsync(user, request.RoleNames);
    }
}

using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;

    public CreateUserCommandHandler(IIdentityService identityService, ICurrentUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.Email);

        if (user is not null)
        {
            throw new FailureException("信箱已被使用，請選擇其他信箱。");
        }

        var currentUser = _currentUser.GetCurrentUser();

        if(!currentUser.Roles.Any(r => r.Name == RoleEnum.Admin.ToString()))
        {
            throw new FailureException("權限不足，無法執行此操作。");
        }

        return await _identityService.CreateUserAsync(
            request.Email,
            request.UserName,
            request.Password,
            request.RoleNames
        );
    }
}

using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.Email);

        if (user is not null)
        {
            throw new FailureException("信箱已被使用，請選擇其他信箱。");
        }

        return await _identityService.CreateUserAsync(
            request.Email,
            request.UserName,
            request.Password,
            request.RoleNames
        );
    }
}

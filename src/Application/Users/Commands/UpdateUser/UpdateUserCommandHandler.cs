using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"找不到使用者（{request.Id}）。");
        }

        user.UserName = request.UserName;
        user.Email = request.Email;

        return await _identityService.UpdateUserAsync(user, request.RoleNames);
    }
}

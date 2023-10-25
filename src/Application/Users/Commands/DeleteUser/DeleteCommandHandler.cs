using Application.Common.Exceptions;
using Application.Common.Identity;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
{
    private readonly IIdentityService _identityService;

    public DeleteCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"找不到使用者（{request.Id}）。");
        }

        return await _identityService.DeleteUserAsync(user);
    }
}

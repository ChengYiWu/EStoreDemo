using MediatR;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    string Id,
    string Email,
    string UserName,
    string[]? RoleNames
) : IRequest<bool>;

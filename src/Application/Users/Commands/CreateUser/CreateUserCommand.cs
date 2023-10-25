using MediatR;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string UserName,
    string Password,
    string[]? RoleNames
) : IRequest<string>;

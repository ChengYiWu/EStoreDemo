using MediatR;

namespace Application.Auths.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<LoginResponse>;


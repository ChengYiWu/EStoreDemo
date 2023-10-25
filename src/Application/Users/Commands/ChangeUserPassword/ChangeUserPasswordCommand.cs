using MediatR;

namespace Application.Users.Commands.ChangeUserPassword;

public record ChangeUserPasswordCommand(
    string Id,
    string OldPassword,
    string NewPassword
) : IRequest<bool>;

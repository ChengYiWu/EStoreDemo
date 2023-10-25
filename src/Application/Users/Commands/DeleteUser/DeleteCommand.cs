using MediatR;

namespace Application.Users.Commands.DeleteUser;

public record DeleteCommand(
    string Id
) : IRequest<bool>;

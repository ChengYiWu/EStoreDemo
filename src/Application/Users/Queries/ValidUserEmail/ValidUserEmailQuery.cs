using MediatR;

namespace Application.Users.Queries.ValidUserEmail;

public record ValidUserEmailQuery(
    string Email
) : IRequest<bool>;

using Application.Users.Queries.GetUsers;
using MediatR;

namespace Application.Users.Queries.GetUser;

public record GetUserQuery(
    string Id
) : IRequest<UserResponse>;

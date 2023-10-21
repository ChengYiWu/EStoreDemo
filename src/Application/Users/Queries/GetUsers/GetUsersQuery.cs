using MediatR;

namespace Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    string? Search,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<IEnumerable<UserResponse>>;


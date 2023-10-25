using MediatR;

namespace Application.Users.Queries.GetUserList;

public record GetUserListQuery(
) : IRequest<IEnumerable<UserForListResponse>>;

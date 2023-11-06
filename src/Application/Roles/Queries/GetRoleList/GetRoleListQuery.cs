
using MediatR;

namespace Application.Roles.Queries.GetRoleList;
public record GetRoleListQuery() : IRequest<RoleListResponse>;



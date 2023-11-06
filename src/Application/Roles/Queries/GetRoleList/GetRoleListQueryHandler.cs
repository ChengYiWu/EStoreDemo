using Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace Application.Roles.Queries.GetRoleList;

public class GetRoleListQueryHandler : IRequestHandler<GetRoleListQuery, RoleListResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetRoleListQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<RoleListResponse> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                [Role].[Id], 
                [Role].[Name]
            FROM [Role]
        ";

        var roleList = await conn.QueryAsync<RoleListItemDTO>(sql);

        return new RoleListResponse
        {
            Items = roleList
        };
    }
}

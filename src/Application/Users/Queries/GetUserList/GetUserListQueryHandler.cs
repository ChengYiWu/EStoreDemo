using Application.Common.Interfaces;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace Application.Users.Queries.GetUserList;

public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, IEnumerable<UserForListResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUserListQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<IEnumerable<UserForListResponse>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection connection = _sqlConnectionFactory.CreateConnection();

        var sql = @$"
            SELECT 
                [User].[Id],
                [User].[UserName]
            FROM [User]
            ORDER BY [User].[Id] ASC
        ";

        return await connection.QueryAsync<UserForListResponse>(sql);
    }
}

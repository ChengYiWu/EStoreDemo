using Application.Common.Interfaces;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                [User].[Id],
                [User].[UserName],
                [User].[Email], 
                [Role].[Id],
                [Role].[Name]
            FROM [User]
            JOIN [UserRole]
                ON [User].[Id] = [UserRole].[UserId]
            JOIN [Role]
                ON [UserRole].[RoleId] = [Role].[Id]
            ORDER BY [User].[Id] ASC
        ";

        // Dapper 自動依照 Select 的 Id 欄位切分 Model，如果 Join 出來的 Table 順序會亂跳的話有可能 Model 會有重複建立的問題，
        // 故可能要使用範例第 8 種方式去 lookup，去判斷物件的 Id 是不是已經出現過了，因為 Dapper 可能會建立相同 Id 但兩個不同物件的情況。
        // Ref: https://gist.github.com/Lobstrosity/1133111
        return await connection.QueryAsync<UserResponse, RoleResponse, UserResponse>(sql, (user, role) =>
        {
            user.Roles.Add(role);
            return user;
        });
    }
}

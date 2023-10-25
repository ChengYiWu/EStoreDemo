using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Utils;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PaginatedList<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection connection = _sqlConnectionFactory.CreateConnection();

        var whereSql = new StringBuilder();

        if (request.Search is not null && string.IsNullOrWhiteSpace(request.Search))
        {
            whereSql.Append(@" AND 
                ( [User].[UserName] LIKE @Search OR [User].[Email] LIKE @Search ) 
            ");
        }

        var sql = @$"
            SELECT COUNT([User].[Id])
            FROM [User]
            {whereSql}

            SELECT 
                [User].[Id],
                [User].[UserName],
                [User].[Email]
            FROM [User]
            {whereSql}
            ORDER BY [User].[Id] ASC
            OFFSET @Offset ROWS
            FETCH NEXT @Next ROWS ONLY
        ";

        (int offset, int next, int pageSize, int pageNumber) = QueryHelper.GetPagingParams(request.PageSize, request.PageNumber);
        var queryParam = new
        {
            Search = "%" + request.Search + "%",
            Offset = offset,
            Next = next
        };

        var queryReader = await connection.QueryMultipleAsync(sql, queryParam);

        var totalCount = await queryReader.ReadSingleAsync<int>();
        var userReponses = await queryReader.ReadAsync<UserResponse>();

        var detailSql = @"
            SELECT 
                [User].[Id] AS [UserId], 
                [Role].[Name]
            FROM [User]
            JOIN [UserRole]
                ON [User].[Id] = [UserRole].[UserId]
            JOIN [Role]
                ON [UserRole].[RoleId] = [Role].[Id]
            WHERE [User].[Id] IN @Ids
        ";

        var detailParam = new
        {
            Ids = userReponses.Select(x => x.Id)
        };

        var userRoles = await connection.QueryAsync(detailSql, detailParam);

        var userRoleLookup = userRoles
            .ToLookup(
                userRole => userRole.UserId,
                userRole => new RoleDTO { Name = userRole.Name }
            );

        foreach (var userResponse in userReponses)
        {
            userResponse.Roles = userRoleLookup[userResponse.Id].ToList();
        }

        return new PaginatedList<UserResponse>(userReponses, totalCount, pageNumber, pageSize);
    }

    private void ThisIsSingleQueryExample()
    {
        //await using SqlConnection connection = _sqlConnectionFactory.CreateConnection();

        //var sql = @"
        //    SELECT 
        //        [User].[Id],
        //        [User].[UserName],
        //        [User].[Email], 
        //        [Role].[Id],
        //        [Role].[Name]
        //    FROM [User]
        //    JOIN [UserRole]
        //        ON [User].[Id] = [UserRole].[UserId]
        //    JOIN [Role]
        //        ON [UserRole].[RoleId] = [Role].[Id]
        //    ORDER BY [User].[Id] ASC
        //";

        // Dapper 自動依照 Select 的 Id 欄位切分 Model，如果 Join 出來的 Table 順序會亂跳的話有可能 Model 會有重複建立的問題，
        // 故可能要使用範例第 8 種方式去 lookup，去判斷物件的 Id 是不是已經出現過了，因為 Dapper 可能會建立相同 Id 但兩個不同物件的情況。
        // Ref: https://gist.github.com/Lobstrosity/1133111
        // Ref: https://stackoverflow.com/questions/7472088/correct-use-of-multimapping-in-dapper
        //return await connection.QueryAsync<UserResponse, RoleDTO, UserResponse>(sql, (user, role) =>
        //{
        //    user.Roles.Add(role);
        //    return user;
        //});
    }
}

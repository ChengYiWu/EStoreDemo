using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Queries.GetUsers;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUserQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<UserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection connection = _sqlConnectionFactory.CreateConnection();

        var querySql = @$"
            SELECT 
                [User].[Id],
                [User].[UserName],
                [User].[Email], 
                [Role].[Id] AS [RoleId],
                [Role].[Name]
            FROM [User]
            LEFT JOIN [UserRole]
                ON [User].[Id] = [UserRole].[UserId]
            LEFT JOIN [Role]
                ON [UserRole].[RoleId] = [Role].[Id]
            WHERE [User].[Id] = @Id
            
        ";

        var param = new { request.Id };

        var queryResult = await connection.QueryAsync<UserResponse, RoleDTO, UserResponse>(
            querySql,
            (user, role) =>
            {
                if (role.Name is not null)
                {
                    user.Roles.Add(role);
                }

                return user;
            },
            param
        );

        var userResult = queryResult
                .GroupBy(user => user.Id)
                .Select(g =>
                {
                    var groupedUser = g.First();
                    groupedUser.Roles = g.Select(user => user.Roles.Single()).ToList();
                    return groupedUser;
                });


        var user = queryResult.SingleOrDefault();

        if (user is null)
        {
            throw new NotFoundException($"找不到使用者（{request.Id}）。");
        }

        return user;
    }
}

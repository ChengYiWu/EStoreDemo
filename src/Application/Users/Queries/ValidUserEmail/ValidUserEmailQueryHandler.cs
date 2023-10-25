using Application.Common.Interfaces;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace Application.Users.Queries.ValidUserEmail;

public class ValidUserEmailQueryHandler : IRequestHandler<ValidUserEmailQuery, bool>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ValidUserEmailQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<bool> Handle(ValidUserEmailQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection conn = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT CASE WHEN EXISTS (SELECT [Id] FROM [User] WHERE [Email] = @Email) 
		            THEN 0
		            ELSE 1 
                END
        ";

        var param = new
        {
            request.Email
        };

        return await conn.QueryFirstAsync<bool>(sql, param);
    }
}

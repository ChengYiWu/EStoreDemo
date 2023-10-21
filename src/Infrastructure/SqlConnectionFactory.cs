using Application.Common.Interfaces;
using Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly ConnectionStringOption _connectionStringOption;

    public SqlConnectionFactory(IOptions<ConnectionStringOption> connectionStringOption)
    {
        _connectionStringOption = connectionStringOption.Value;
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionStringOption.DefaultConnection);
    }
}

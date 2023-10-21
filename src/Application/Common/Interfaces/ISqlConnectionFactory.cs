using Microsoft.Data.SqlClient;

namespace Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}

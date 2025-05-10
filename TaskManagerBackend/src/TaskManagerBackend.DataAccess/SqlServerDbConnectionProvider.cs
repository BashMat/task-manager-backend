#region Usings

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerBackend.Common;

#endregion

namespace TaskManagerBackend.DataAccess;

public class SqlServerDbConnectionProvider : IDbConnectionProvider<SqlConnection>
{
    private readonly IConfiguration _configuration;

    public SqlServerDbConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
    }
}
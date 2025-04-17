#region Usings

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerBackend.Common;

#endregion

namespace TaskManagerBackend.DataAccess;

public class SqlServerDbConnectionProvider : IDbConnectionProvider
{
    private readonly IConfiguration _configuration;

    public SqlServerDbConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IDbConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
    }
}
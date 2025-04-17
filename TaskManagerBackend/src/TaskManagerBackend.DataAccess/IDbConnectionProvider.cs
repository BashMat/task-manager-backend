using System.Data;

namespace TaskManagerBackend.DataAccess;

public interface IDbConnectionProvider
{
    IDbConnection GetConnection();
}
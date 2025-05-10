using System.Data;

namespace TaskManagerBackend.DataAccess;

public interface IDbConnectionProvider<out TConnection> where TConnection: IDbConnection 
{
    TConnection GetConnection();
}
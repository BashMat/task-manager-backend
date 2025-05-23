#region Usings

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionProvider<SqlConnection> _dbConnectionProvider;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnectionProvider<SqlConnection> dbConnectionProvider,
                          ILogger<UserRepository> logger)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _logger = logger;
    }

    public async Task<UserPasswordData?> GetUserPasswordData(string logInData)
    {
        _logger.LogInformation("Starting getting user password data");

        await using SqlConnection connection = _dbConnectionProvider.GetConnection();
            
        UserPasswordData? data = await connection.QueryFirstOrDefaultAsync<UserPasswordData>(
                            @"select [Id], [PasswordHash], [PasswordSalt] from [User] 
where [UserName] = @LogInData or [Email] = @LogInData",
                            new { LogInData = logInData });

        _logger.LogInformation("Finishing getting user password data");
        
        return data;
    }

    public async Task<bool> CheckIfUserExistsById(int id)
    {
        _logger.LogInformation("Starting checking if user exists by ID");
        
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();
            
        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [Id] = @Id",
                             new { Id = id });
        
        _logger.LogInformation("Finishing checking if user exists by ID");

        return user is not null;
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
    {
        _logger.LogInformation("Starting checking if user exists by user name or email");
        
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [UserName] = @UserName or [Email] = @Email",
                             new { UserName = userName, Email = email });
        
        _logger.LogInformation("Finishing checking if user exists by user name or email");
            
        return user is not null;
    }

    public async Task InsertUser(Domain.Users.User user)
    {
        _logger.LogInformation("Starting inserting user data");
        
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        await connection.ExecuteAsync(@"insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values 
(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)",
                                      user);
        
        _logger.LogInformation("Finishing inserting user data");
    }
}
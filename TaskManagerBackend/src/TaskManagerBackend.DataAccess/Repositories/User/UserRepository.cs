using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.Common;
using TaskManagerBackend.Dto.User;
using Models = TaskManagerBackend.Domain.Models;

namespace TaskManagerBackend.DataAccess.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IConfiguration configuration,
                          ILogger<UserRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserPasswordData?> GetUserPasswordData(string logInData)
    {
        _logger.LogInformation("Starting getting user password data");
        
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
            
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
        
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
            
        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [Id] = @Id",
                             new { Id = id });
        
        _logger.LogInformation("Finishing checking if user exists by ID");

        return user is not null;
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
    {
        _logger.LogInformation("Starting checking if user exists by user name or email");
        
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [UserName] = @UserName or [Email] = @Email",
                             new { UserName = userName, Email = email });
        
        _logger.LogInformation("Finishing checking if user exists by user name or email");
            
        return user is not null;
    }

    public async Task Insert(Models.User user)
    {
        _logger.LogInformation("Starting inserting user data");
        
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        await connection.ExecuteAsync(@"insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values 
(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)",
                                      user);
        
        _logger.LogInformation("Finishing inserting user data");
    }
}
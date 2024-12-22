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
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
            
        UserPasswordData? data = await connection.QueryFirstOrDefaultAsync<UserPasswordData>(
                            @"select [Id], [PasswordHash], [PasswordSalt] from [User] 
where [UserName] = @LogInData or [Email] = @LogInData",
                            new { LogInData = logInData });

        return data;
    }

    public async Task<bool> CheckIfUserExistsById(int id)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));
            
        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [Id] = @Id",
                             new { Id = id });

        return user is not null;
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        DynamicParameters parameters = new();
        parameters.Add("@UserName", userName);
        parameters.Add("@Email", email);

        UserNameAndEmailData? user = await connection.QueryFirstOrDefaultAsync<UserNameAndEmailData>(
                             "select [UserName], [Email] from [User] where [UserName] = @UserName or [Email] = @Email",
                             parameters);
            
        return user is not null;
    }

    public async Task Insert(Models.User user)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        await connection.ExecuteAsync(@"insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values 
(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)",
                                      user);
    }
}
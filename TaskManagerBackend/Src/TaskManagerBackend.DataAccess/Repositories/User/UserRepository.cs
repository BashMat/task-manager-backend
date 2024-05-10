using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.Common;
using Models = TaskManagerBackend.Domain.Models;

namespace TaskManagerBackend.DataAccess.Repositories.User
{
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

        public async Task<Tuple<int, byte[], byte[]>?> GetUserPasswordData(string logInData)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.DefaultConnection));

            try
            {
                (int id, byte[] passwordHash, byte[] passwordSalt) = await connection.QueryFirstAsync<(int, byte[], byte[])>(
                    "select [Id], [PasswordHash], [PasswordSalt] from [User] where [UserName] = @LogInData or [Email] = @LogInData",
                    new { LogInData = logInData });
                return new(id, passwordHash, passwordSalt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> CheckIfUserExistsById(int id)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.DefaultConnection));

            try
            {
                await connection.QueryFirstAsync(
                    "select [UserName], [Email] from [User] where [Id] = @Id",
                    new { Id = id });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.DefaultConnection));

            DynamicParameters parameters = new();
            parameters.Add("@UserName", userName);
            parameters.Add("@Email", email);

            try
            {
                await connection.QueryFirstAsync(
                    "select [UserName], [Email] from [User] where [UserName] = @UserName or [Email] = @Email",
                    parameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task Insert(Models.User user)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.DefaultConnection));

            await connection.ExecuteAsync(
                "insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values " +
                "(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)", user);
        }
    }
}
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models = TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Tuple<int, byte[], byte[]>?> GetUserPasswordData(string logInData)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                (int id, byte[] passwordHash, byte[] passwordSalt) = await connection.QueryFirstAsync<(int, byte[], byte[])>(
                    "select [Id], [PasswordHash], [PasswordSalt] from [User] where [UserName] = @LogInData or [Email] = @LogInData",
                    new { LogInData = logInData });
                return new(id, passwordHash, passwordSalt);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> CheckIfUserExistsById(int id)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                await connection.QueryFirstAsync(
                    "select [UserName], [Email] from [User] where [Id] = @Id",
                    new { Id = id });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

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
                return false;
            }
        }

        public async Task Insert(Models.User user)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(
                "insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values " +
                "(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)", user);

            return;
        }
    }
}